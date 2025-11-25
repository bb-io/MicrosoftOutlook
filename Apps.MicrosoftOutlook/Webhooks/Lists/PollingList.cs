using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Polling;
using Apps.MicrosoftOutlook.Webhooks.Memory;
using Microsoft.Graph.Models.ODataErrors;
using Microsoft.Graph.Models;
using Apps.MicrosoftOutlook.Webhooks.Payload;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Apps.MicrosoftOutlook.Utils;

namespace Apps.MicrosoftOutlook.Webhooks.Lists;

[PollingEventList]
public class PollingList(InvocationContext invocationContext) : BaseInvocable(invocationContext)
{
    [PollingEvent("On emails received", "This webhook is triggered when new emails are received.")]
    public async Task<PollingEventResponse<LastEmailMemory, ReceivedMessagesResponse>> OnEmailsReceived(
        PollingEventRequest<LastEmailMemory> request,
        [PollingEventParameter] PollingInput input)
    {
        if (request.Memory == null)
        {
            GetNewReceivedEmails(null, input, false, out var newLastDateTime);
            return new()
            {
                FlyBird = false,
                Memory = new() { LastEmailDateTime = newLastDateTime }
            };
        }

        var receivedEmails = GetNewReceivedEmails(request.Memory.LastEmailDateTime, input, false, out var newDeltaToken);

        if (receivedEmails.Count() == 0)
        {
            return new()
            {
                FlyBird = false,
                Memory = new() { LastEmailDateTime = newDeltaToken }
            };
        }

        return new()
        {
            FlyBird = true,
            Memory = new() { LastEmailDateTime = newDeltaToken },
            Result = new() { Emails = receivedEmails.ToList() }
        };
    }

    [PollingEvent("On emails with files attached received", "This webhook is triggered when emails with file attachments are received.")]
    public async Task<PollingEventResponse<LastEmailMemory, ReceivedMessagesResponse>> OnEmailsWithAttachmentsReceived(
        PollingEventRequest<LastEmailMemory> request, [PollingEventParameter] PollingInput input)
    {
        if (request.Memory == null)
        {
            GetNewReceivedEmails(null, input, false, out var newLastDateTime);
            return new()
            {
                FlyBird = false,
                Memory = new() { LastEmailDateTime = newLastDateTime }
            };
        }

        var receivedEmails = GetNewReceivedEmails(request.Memory.LastEmailDateTime, input, true, out var newDeltaToken);

        if (receivedEmails.Count() == 0)
        {
            return new()
            {
                FlyBird = false,
                Memory = new() { LastEmailDateTime = newDeltaToken }
            };
        }

        return new()
        {
            FlyBird = true,
            Memory = new() { LastEmailDateTime = newDeltaToken },
            Result = new() { Emails = receivedEmails.ToList() }
        };
    }

    private IEnumerable<ReceivedMessageDto> GetNewReceivedEmails(DateTime? previousLastDateTime, PollingInput input, bool withAttachments, out DateTime newLastDateTime)
    {
        var client = new MicrosoftOutlookClient(InvocationContext.AuthenticationCredentialsProviders);
        MessageCollectionResponse? messages;
        var messagesList = new List<Message>();
        var startDateTime = (previousLastDateTime ?? DateTime.UtcNow.AddDays(-3)).ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");
        var requestFilter = $"sentDateTime gt {startDateTime}";
        var skipMessagesAmount = 0;
        try
        {
            do
            {
                if (input.MailFolderId == null)
                    messages = ErrorHandler.ExecuteWithErrorHandlingAsync(() => client.Me.Messages.GetAsync(requestConfiguration =>
                    {
                        requestConfiguration.QueryParameters.Filter = requestFilter;
                        requestConfiguration.QueryParameters.Skip = skipMessagesAmount;
                    })).Result;
                else
                    messages = ErrorHandler.ExecuteWithErrorHandlingAsync(() => client.Me.MailFolders[input.MailFolderId].Messages.GetAsync(requestConfiguration =>
                    {
                        requestConfiguration.QueryParameters.Filter = requestFilter;
                        requestConfiguration.QueryParameters.Skip = skipMessagesAmount;
                    })).Result;
                messagesList.AddRange(messages.Value);
                skipMessagesAmount += 10;
            } while (messages.OdataNextLink != null);
        }
        catch (ODataError error)
        {
            throw new PluginMisconfigurationException(error.Error.Message);
        }

        var messagesDtos = messagesList.Where(x => withAttachments ? MessageWithSenderAndAttachmentsFilter(client, x, input) : MessageWithSenderFilter(x, input)).Select(m => new ReceivedMessageDto(m)).ToList();
        newLastDateTime = messagesDtos.Any() ? messagesDtos.Max(m => m.SentDateTime) : previousLastDateTime ?? DateTime.UtcNow;
        return messagesDtos;
    }

    private bool MessageWithSenderAndAttachmentsFilter(MicrosoftOutlookClient client, Message message, PollingInput input)
    {
        if(!message.HasAttachments.HasValue || !message.HasAttachments.Value)
            return false;

        var attachments = ErrorHandler.ExecuteWithErrorHandlingAsync(() => client.Me.Messages[message.Id].Attachments.GetAsync()).Result;
        var fileAttachments = attachments?.Value?.Where(a => a is FileAttachment);

        if (fileAttachments == null || !fileAttachments.Any())
            return false;

        if (input.Email is not null && message?.Sender?.EmailAddress?.Address != input.Email)
            return false;

        var receiverEmails = message?.ToRecipients?.Select(r => r.EmailAddress?.Address);
        if (input.ReceiverEmail is not null && receiverEmails is not null && receiverEmails.All(x => x != input.ReceiverEmail))
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

        if (input.SubjectContains is not null && message?.Subject?.Contains(input.SubjectContains, StringComparison.InvariantCultureIgnoreCase) != true)
            return false;

        if (input.ContentContains is not null && message?.Body?.Content?.Contains(input.ContentContains, StringComparison.InvariantCultureIgnoreCase) != true)
            return false;

        return true;
    }
}

