using Apps.MicrosoftOutlook.Utils;
using Apps.MicrosoftOutlook.Webhooks.Memory;
using Apps.MicrosoftOutlook.Webhooks.Payload;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common.Polling;
using Microsoft.Graph.Models;

namespace Apps.MicrosoftOutlook.Webhooks.Lists;

[PollingEventList]
public class PollingList(InvocationContext invocationContext) : BaseInvocable(invocationContext)
{
    private const int MessagesPageSize = 50;

    [PollingEvent("On emails received", "This webhook is triggered when new emails are received.")]
    public Task<PollingEventResponse<LastEmailMemory, ReceivedMessagesResponse>> OnEmailsReceived(
        PollingEventRequest<LastEmailMemory> request,
        [PollingEventParameter] PollingInput input)
    {
        return PollNewEmails(request, input, withAttachments: false);
    }

    [PollingEvent("On emails with files attached received", "This webhook is triggered when emails with file attachments are received.")]
    public Task<PollingEventResponse<LastEmailMemory, ReceivedMessagesResponse>> OnEmailsWithAttachmentsReceived(
        PollingEventRequest<LastEmailMemory> request, [PollingEventParameter] PollingInput input)
    {
        return PollNewEmails(request, input, withAttachments: true);
    }

    private async Task<PollingEventResponse<LastEmailMemory, ReceivedMessagesResponse>> PollNewEmails(
        PollingEventRequest<LastEmailMemory> request, PollingInput input, bool withAttachments)
    {
        var utcNow = DateTime.UtcNow;

        if (request.Memory == null)
        {
            return new()
            {
                FlyBird = false,
                Memory = LastEmailMemoryTracker.CreateInitial(utcNow)
            };
        }

        var memory = LastEmailMemoryTracker.Normalize(request.Memory, utcNow);
        var receivedMessages = await GetMessagesReceivedSince(memory, input.MailFolderId, withAttachments);
        var unseenEmails = LastEmailMemoryTracker.ExcludeAlreadySeen(
            receivedMessages.Select(message => new ReceivedMessageDto(message)), memory);
        var unseenIds = unseenEmails.Select(email => email.MessageId).ToHashSet();

        var matchingEmails = receivedMessages
            .Where(message => unseenIds.Contains(message.Id) && MatchesInput(message, input, withAttachments))
            .Select(message => new ReceivedMessageDto(message))
            .ToList();

        return new()
        {
            FlyBird = matchingEmails.Count > 0,
            Memory = LastEmailMemoryTracker.Advance(memory, unseenEmails),
            Result = matchingEmails.Count > 0 ? new() { Emails = matchingEmails } : null
        };
    }

    private async Task<List<Message>> GetMessagesReceivedSince(LastEmailMemory memory, string? mailFolderId,
        bool withAttachments)
    {
        var client = new MicrosoftOutlookClient(InvocationContext.AuthenticationCredentialsProviders);
        var requestFilter = LastEmailMemoryTracker.BuildNewEmailsFilter(memory);
        var messagesList = new List<Message>();
        var skipMessagesAmount = 0;
        MessageCollectionResponse? messages;

        do
        {
            messages = await GetMessagesPage(client, mailFolderId, requestFilter, skipMessagesAmount, withAttachments);
            if (messages?.Value != null)
                messagesList.AddRange(messages.Value);

            skipMessagesAmount += MessagesPageSize;
        }
        while (messages?.OdataNextLink != null);

        return messagesList
            .GroupBy(message => message.Id)
            .Select(group => group.First())
            .ToList();
    }

    private bool MatchesInput(Message message, PollingInput input, bool withAttachments) =>
        withAttachments
            ? MessageWithSenderAndAttachmentsFilter(message, input)
            : MessageWithSenderFilter(message, input);

    private static Task<MessageCollectionResponse?> GetMessagesPage(MicrosoftOutlookClient client, string? mailFolderId,
        string requestFilter, int skipMessagesAmount, bool withAttachments)
    {
        string[]? expand = withAttachments ? ["attachments($select=id)"] : null;
        string[] orderBy = [LastEmailMemoryTracker.OrderBy];

        if (mailFolderId == null)
            return ErrorHandler.ExecuteWithErrorHandlingAsync(() => client.Me.Messages.GetAsync(configuration =>
            {
                configuration.QueryParameters.Filter = requestFilter;
                configuration.QueryParameters.Top = MessagesPageSize;
                configuration.QueryParameters.Skip = skipMessagesAmount;
                configuration.QueryParameters.Orderby = orderBy;
                configuration.QueryParameters.Expand = expand;
            }));

        return ErrorHandler.ExecuteWithErrorHandlingAsync(() => client.Me.MailFolders[mailFolderId].Messages.GetAsync(configuration =>
        {
            configuration.QueryParameters.Filter = requestFilter;
            configuration.QueryParameters.Top = MessagesPageSize;
            configuration.QueryParameters.Skip = skipMessagesAmount;
            configuration.QueryParameters.Orderby = orderBy;
            configuration.QueryParameters.Expand = expand;
        }));
    }

    private bool MessageWithSenderAndAttachmentsFilter(Message message, PollingInput input)
    {
        if (!message.HasAttachments.HasValue || !message.HasAttachments.Value)
            return false;

        if (message.Attachments?.Any(a => a is FileAttachment) != true)
            return false;

        if (input.Email is not null && message?.Sender?.EmailAddress?.Address != input.Email)
            return false;

        var receiverEmails = message?.ToRecipients?.Select(r => r.EmailAddress?.Address);
        if (input.ReceiverEmail is not null && receiverEmails is not null && receiverEmails.All(x => x != input.ReceiverEmail))
            return false;

        if (!HasMatchingCcRecipient(message, input.CcEmail))
            return false;

        if (input.SubjectContains is not null && message?.Subject?.Contains(input.SubjectContains, StringComparison.InvariantCultureIgnoreCase) != true)
            return false;

        if (input.ContentContains is not null && message?.Body?.Content?.Contains(input.ContentContains, StringComparison.InvariantCultureIgnoreCase) != true)
            return false;

        return true;
    }

    private bool MessageWithSenderFilter(Message message, PollingInput input)
    {
        if (message == null)
            return false;

        if (input.Email is not null && message?.Sender?.EmailAddress?.Address != input.Email)
            return false;

        var receiverEmails = message?.ToRecipients?.Select(r => r.EmailAddress?.Address);
        if (input.ReceiverEmail is not null && receiverEmails is not null && receiverEmails.All(x => x != input.ReceiverEmail))
            return false;

        if (!HasMatchingCcRecipient(message, input.CcEmail))
            return false;

        if (input.SubjectContains is not null && message?.Subject?.Contains(input.SubjectContains, StringComparison.InvariantCultureIgnoreCase) != true)
            return false;

        if (input.ContentContains is not null && message?.Body?.Content?.Contains(input.ContentContains, StringComparison.InvariantCultureIgnoreCase) != true)
            return false;

        return true;
    }

    private bool HasMatchingCcRecipient(Message message, string? ccEmail)
    {
        if (string.IsNullOrWhiteSpace(ccEmail))
            return true;

        var normalizedCcEmail = ccEmail.Trim();
        var ccRecipients = message.CcRecipients?
            .Select(r => r.EmailAddress?.Address?.Trim())
            .Where(x => !string.IsNullOrWhiteSpace(x));

        return ccRecipients?.Any(x => string.Equals(x, normalizedCcEmail, StringComparison.InvariantCultureIgnoreCase)) == true;
    }
}

