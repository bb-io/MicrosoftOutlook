using Apps.MicrosoftOutlook.Utils;
using Apps.MicrosoftOutlook.Webhooks.Memory;
using Apps.MicrosoftOutlook.Webhooks.Payload;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common.Polling;
using Microsoft.Graph.Models;
using Microsoft.Graph.Models.ODataErrors;
using System.Globalization;

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
            var initialEmails = GetNewReceivedEmails(
                previousLastDateTime: null,
                input: input,
                withAttachments: false,
                out var initLastDateTime,
                lastIdsAtLastDateTime: null);

            return new()
            {
                FlyBird = false,
                Memory = new()
                {
                    LastEmailDateTime = initLastDateTime,
                    LastMessageIdsAtLastDateTime = initialEmails
                        .Where(x => x.SentDateTime == initLastDateTime)
                        .Select(x => x.MessageId)
                        .Distinct()
                        .ToList()
                }
            };
        }

        var receivedEmails = GetNewReceivedEmails(
            previousLastDateTime: request.Memory.LastEmailDateTime,
            input: input,
            withAttachments: false,
            out var nextLastDateTime,
            lastIdsAtLastDateTime: request.Memory.LastMessageIdsAtLastDateTime);

        if (!receivedEmails.Any())
        {
            return new()
            {
                FlyBird = false,
                Memory = new()
                {
                    LastEmailDateTime = nextLastDateTime,
                    LastMessageIdsAtLastDateTime = request.Memory.LastMessageIdsAtLastDateTime ?? new()
                }
            };
        }

        var idsAtMaxTime = receivedEmails
            .Where(x => x.SentDateTime == nextLastDateTime)
            .Select(x => x.MessageId)
            .Distinct()
            .ToList();

        var nextIds = nextLastDateTime > request.Memory.LastEmailDateTime
            ? idsAtMaxTime
            : (request.Memory.LastMessageIdsAtLastDateTime ?? new()).Union(idsAtMaxTime).Distinct().ToList();

        return new()
        {
            FlyBird = true,
            Memory = new()
            {
                LastEmailDateTime = nextLastDateTime,
                LastMessageIdsAtLastDateTime = nextIds
            },
            Result = new() { Emails = receivedEmails.ToList() }
        };
    }

    [PollingEvent("On emails with files attached received", "This webhook is triggered when emails with file attachments are received.")]
    public async Task<PollingEventResponse<LastEmailMemory, ReceivedMessagesResponse>> OnEmailsWithAttachmentsReceived(
        PollingEventRequest<LastEmailMemory> request, [PollingEventParameter] PollingInput input)
    {
        if (request.Memory == null)
        {
            var initialEmails = GetNewReceivedEmails(
                previousLastDateTime: null,
                input: input,
                withAttachments: true,
                out var initLastDateTime,
                lastIdsAtLastDateTime: null);

            return new()
            {
                FlyBird = false,
                Memory = new()
                {
                    LastEmailDateTime = initLastDateTime,
                    LastMessageIdsAtLastDateTime = initialEmails
                        .Where(x => x.SentDateTime == initLastDateTime)
                        .Select(x => x.MessageId)
                        .Distinct()
                        .ToList()
                }
            };
        }

        var receivedEmails = GetNewReceivedEmails(
            previousLastDateTime: request.Memory.LastEmailDateTime,
            input: input,
            withAttachments: true,
            out var nextLastDateTime,
            lastIdsAtLastDateTime: request.Memory.LastMessageIdsAtLastDateTime);

        if (!receivedEmails.Any())
        {
            return new()
            {
                FlyBird = false,
                Memory = new()
                {
                    LastEmailDateTime = nextLastDateTime,
                    LastMessageIdsAtLastDateTime = request.Memory.LastMessageIdsAtLastDateTime ?? new()
                }
            };
        }

        var idsAtMaxTime = receivedEmails
            .Where(x => x.SentDateTime == nextLastDateTime)
            .Select(x => x.MessageId)
            .Distinct()
            .ToList();

        var nextIds = nextLastDateTime > request.Memory.LastEmailDateTime
            ? idsAtMaxTime
            : (request.Memory.LastMessageIdsAtLastDateTime ?? new()).Union(idsAtMaxTime).Distinct().ToList();

        return new()
        {
            FlyBird = true,
            Memory = new()
            {
                LastEmailDateTime = nextLastDateTime,
                LastMessageIdsAtLastDateTime = nextIds
            },
            Result = new() { Emails = receivedEmails.ToList() }
        };
    }

    private IEnumerable<ReceivedMessageDto> GetNewReceivedEmails(DateTime? previousLastDateTime, PollingInput input, bool withAttachments, out DateTime newLastDateTime,
            List<string>? lastIdsAtLastDateTime)
    {
        var client = new MicrosoftOutlookClient(InvocationContext.AuthenticationCredentialsProviders);
        MessageCollectionResponse? messages;

        var messagesList = new List<Message>();

        var startDateTime = (previousLastDateTime ?? DateTime.UtcNow.AddDays(-3))
            .ToUniversalTime()
            .ToString("o", CultureInfo.InvariantCulture);

        var requestFilter = $"sentDateTime gt {startDateTime}";

        const int pageSize = 50;
        var skipMessagesAmount = 0;

        try
        {
            do
            {
                if (input.MailFolderId == null)
                {
                    messages = ErrorHandler.ExecuteWithErrorHandlingAsync(() =>
                        client.Me.Messages.GetAsync(requestConfiguration =>
                        {
                            requestConfiguration.QueryParameters.Filter = requestFilter;
                            requestConfiguration.QueryParameters.Top = pageSize;
                            requestConfiguration.QueryParameters.Skip = skipMessagesAmount;
                            requestConfiguration.QueryParameters.Orderby = new[] { "sentDateTime asc" };
                        })).Result;
                }
                else
                {
                    messages = ErrorHandler.ExecuteWithErrorHandlingAsync(() =>
                        client.Me.MailFolders[input.MailFolderId].Messages.GetAsync(requestConfiguration =>
                        {
                            requestConfiguration.QueryParameters.Filter = requestFilter;
                            requestConfiguration.QueryParameters.Top = pageSize;
                            requestConfiguration.QueryParameters.Skip = skipMessagesAmount;
                            requestConfiguration.QueryParameters.Orderby = new[] { "sentDateTime asc" };
                        })).Result;
                }

                if (messages?.Value != null)
                    messagesList.AddRange(messages.Value);

                skipMessagesAmount += pageSize;
            }
            while (messages?.OdataNextLink != null);
        }
        catch (ODataError error)
        {
            throw new PluginMisconfigurationException(error.Error.Message);
        }

        var filtered = messagesList.Where(x =>
                withAttachments
                    ? MessageWithSenderAndAttachmentsFilter(client, x, input)
                    : MessageWithSenderFilter(x, input))
            .ToList();

        filtered = filtered
            .GroupBy(m => m.Id)
            .Select(g => g.First())
            .ToList();

        var dtos = filtered
            .Select(m => new ReceivedMessageDto(m))
            .ToList();

        if (previousLastDateTime.HasValue && lastIdsAtLastDateTime != null && lastIdsAtLastDateTime.Count > 0)
        {
            dtos = dtos
                .Where(d =>
                    d.SentDateTime > previousLastDateTime.Value ||
                    (d.SentDateTime == previousLastDateTime.Value && !lastIdsAtLastDateTime.Contains(d.MessageId)))
                .ToList();
        }

        newLastDateTime = dtos.Any()
            ? dtos.Max(m => m.SentDateTime)
            : previousLastDateTime ?? DateTime.UtcNow;

        return dtos;
    }

    private bool MessageWithSenderAndAttachmentsFilter(MicrosoftOutlookClient client, Message message, PollingInput input)
    {
        if (!message.HasAttachments.HasValue || !message.HasAttachments.Value)
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

