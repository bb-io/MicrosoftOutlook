using Apps.MicrosoftOutlook.Dtos;
using Apps.MicrosoftOutlook.Models.Mail.Requests;
using Apps.MicrosoftOutlook.Models.Mail.Responses;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using Blackbird.Applications.Sdk.Utils.Extensions.Files;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using Blackbird.Applications.Sdk.Common.Invocation;
using Apps.MicrosoftOutlook.Utils;
using Blackbird.Applications.Sdk.Common.Exceptions;

namespace Apps.MicrosoftOutlook.Actions;

[ActionList]
public class MailActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient) : BaseInvocable(invocationContext)
{
    MicrosoftOutlookClient outlookClient = new MicrosoftOutlookClient(invocationContext.AuthenticationCredentialsProviders);

    IFileManagementClient fileManagementClient = fileManagementClient;
    #region GET

    [Action("List most recent messages", Description = "List messages received during past hours. If number of " +
                                                             "hours is not specified, messages received during past 24 " +
                                                             "hours are listed. To retrieve messages from specific mail " +
                                                             "folder, specify mail folder.")]
    public async Task<ListRecentMessagesResponse> ListRecentMessages(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
        [ActionParameter] ListRecentMessagesRequest request)
    {
        if (!int.TryParse(request.Hours?.ToString(), out var intHours))
            throw new PluginMisconfigurationException($"Invalid Hours value: {request.Hours} must be an integer value. Please check the input hours.");

        MessageCollectionResponse? messages;
        var messagesList = new List<Message>();
        var startDateTime = (DateTime.Now - TimeSpan.FromHours(request.Hours ?? 24)).ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");
        var requestFilter = $"sentDateTime ge {startDateTime}";
        var skipMessagesAmount = 0;
        do
        {
            if (request.MailFolderId == null)
                messages = await ErrorHandler.ExecuteWithErrorHandlingAsync(async () => await outlookClient.Me.Messages.GetAsync(requestConfiguration =>
                {
                    requestConfiguration.QueryParameters.Filter = requestFilter;
                    requestConfiguration.QueryParameters.Skip = skipMessagesAmount;
                }));
            else
                messages = await ErrorHandler.ExecuteWithErrorHandlingAsync(async () => await outlookClient.Me.MailFolders[request.MailFolderId].Messages.GetAsync(requestConfiguration =>
                {
                    requestConfiguration.QueryParameters.Filter = requestFilter;
                    requestConfiguration.QueryParameters.Skip = skipMessagesAmount;
                }));
            messagesList.AddRange(messages.Value);
            skipMessagesAmount += 10;
        } while (messages.OdataNextLink != null);

        var messagesDto = messagesList.Select(m => new MessageDto(m));
        return new ListRecentMessagesResponse
        {
            Messages = messagesDto
        };
    }

    [Action("Get message", Description = "Retrieve a message from your mailbox.")]
    public async Task<MessageDto> GetMessage(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
        [ActionParameter] GetMessageRequest request)
    {
        var message = await ErrorHandler.ExecuteWithErrorHandlingAsync(async () => await outlookClient.Me.Messages[request.MessageId].GetAsync());
        var messageDto = new MessageDto(message);
        return messageDto;
    }

    [Action("List attached files", Description = "Retrieve a list of files attached to a message.")]
    public async Task<ListAttachmentsResponse> ListAttachedFiles(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
        [ActionParameter] ListAttachedFilesRequest request)
    {
        var attachments = await ErrorHandler.ExecuteWithErrorHandlingAsync(async () => await outlookClient.Me.Messages[request.MessageId].Attachments.GetAsync());

        var fileAttachments = attachments?.Value?.Where(a => a is FileAttachment) ?? Enumerable.Empty<Microsoft.Graph.Models.Attachment>();

        var fileAttachmentsDto = fileAttachments.Select(a => new FileAttachmentDto((FileAttachment)a, fileManagementClient));
        return new ListAttachmentsResponse
        {
            Attachments = fileAttachmentsDto
        };
    }

    [Action("List mail folders", Description = "Retrieve a list of mail folders.")]
    public async Task<ListMailFoldersResponse> ListMailFolders(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders)
    {
        var mailFolders = await ErrorHandler.ExecuteWithErrorHandlingAsync(async () => await outlookClient.Me.MailFolders.GetAsync());
        var mailFoldersDto = mailFolders.Value.Select(f => new MailFolderDto(f));
        return new ListMailFoldersResponse
        {
            MailFolders = mailFoldersDto
        };
    }

    #endregion

    #region POST

    [Action("Create draft message", Description = "Create a draft of a new message. The body of the message can " +
                                                        "be in html format or a plain string.")]
    public async Task<MessageDto> CreateDraftMessage(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
        [ActionParameter] CreateMessageRequest request)
    {
        var requestBody = new Message
        {
            From = string.IsNullOrEmpty(request.SenderEmail) ? null : new Recipient() { EmailAddress = new EmailAddress() { Address = request.SenderEmail } },
            Subject = request.Subject,
            Body = new ItemBody { ContentType = BodyType.Html, Content = request.Content },
            ToRecipients = new List<Recipient>(request.RecipientEmails
                .Select(email => new Recipient { EmailAddress = new EmailAddress { Address = email } }))
        };
        var message = await ErrorHandler.ExecuteWithErrorHandlingAsync(async () => await outlookClient.Me.Messages.PostAsync(requestBody));
        var messageDto = new MessageDto(message);
        return messageDto;
    }

    [Action("Forward message", Description = "Forward a message.")]
    public async Task ForwardMessage(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
        [ActionParameter] ForwardMessageRequest request)
    {
        var requestBody = new Microsoft.Graph.Me.Messages.Item.Forward.ForwardPostRequestBody
        {
            Comment = request.Comment,
            ToRecipients = new List<Recipient>(request.RecipientEmails
                .Select(email => new Recipient { EmailAddress = new EmailAddress { Address = email } }))
        };
        await ErrorHandler.ExecuteWithErrorHandlingAsync(async () => await outlookClient.Me.Messages[request.MessageId].Forward.PostAsync(requestBody));
    }

    [Action("Reply to a message", Description = "Reply to the sender of a message.")]
    public async Task ReplyToMessage(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
        [ActionParameter] ReplyToMessageRequest request)
    {
        var requestBody = new Microsoft.Graph.Me.Messages.Item.Reply.ReplyPostRequestBody
        {
            Comment = request.Comment
        };
        await ErrorHandler.ExecuteWithErrorHandlingAsync(async () => await outlookClient.Me.Messages[request.MessageId].Reply.PostAsync(requestBody));
    }

    [Action("Send draft message", Description = "Send an existing draft message.")]
    public async Task SendDraftMessage(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
        [ActionParameter] SendDraftMessageRequest request)
    {
        if (request == null)
        {
            throw new PluginMisconfigurationException("Input cannot be null. Please check your input and try again");
        }

        if (string.IsNullOrEmpty(request.MessageId))
        {
            throw new PluginMisconfigurationException("Message ID is required to send a draft message. Please check your input and try again");
        }

        await ErrorHandler.ExecuteWithErrorHandlingAsync(async () => await outlookClient.Me.Messages[request.MessageId].Send.PostAsync());
    }

    [Action("Send new message", Description = "Send newly created message.")]
    public async Task SendNewMessage(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
        [ActionParameter] SendNewMessageRequest request)
    {
        if (request.RecipientEmails == null || !request.RecipientEmails.Any())
        {
            throw new PluginMisconfigurationException("At least one recipient email is required. Please check your input and try again");
        }

        if (string.IsNullOrEmpty(request.Subject))
        {
            throw new PluginMisconfigurationException("The email subject is required. Please check your input and try again");
        }

        var requestBody = new Microsoft.Graph.Me.SendMail.SendMailPostRequestBody
        {
            Message = new Message
            {
                From = string.IsNullOrEmpty(request.SenderEmail) ? null : new Recipient() { EmailAddress = new EmailAddress() { Address = request.SenderEmail.Trim() } },
                Subject = request.Subject,
                Body = new ItemBody { ContentType = BodyType.Html, Content = request.Content },
                ToRecipients = request.RecipientEmails.Select(email => new Recipient { EmailAddress = new EmailAddress { Address = email.Trim() } })
                    .ToList()
            }
        };
        
        await ErrorHandler.ExecuteWithErrorHandlingAsync(async () => await outlookClient.Me.SendMail.PostAsync(requestBody));
    }
 
    [Action("Attach file to draft message", Description = "Attach file to a draft message.")]
    public async Task<FileAttachmentDto> AttachFileToDraftMessage(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
        [ActionParameter] AttachFileToDraftMessageRequest request)
    {
        const int threeMegabytesInBytes = 3145728;
        var attachment = new FileAttachment();
        var file = await fileManagementClient.DownloadAsync(request.File);
        var fileBytes = await file.GetByteData();

        if (fileBytes.LongLength < threeMegabytesInBytes)
        {
            var requestBody = new FileAttachment
            {
                Name = request.File.Name,
                ContentBytes = fileBytes,
                ContentType = request.File.ContentType
            };

            attachment = await ErrorHandler.ExecuteWithErrorHandlingAsync(async () => await outlookClient.Me.Messages[request.MessageId].Attachments.PostAsync(requestBody)) as FileAttachment;
        }
        else
        {
            const int chunkSize = 2949120;

            var requestBody = new Microsoft.Graph.Me.Messages.Item.Attachments.CreateUploadSession.CreateUploadSessionPostRequestBody
            {
                AttachmentItem = new AttachmentItem
                {
                    AttachmentType = AttachmentType.File,
                    Name = request.File.Name,
                    Size = fileBytes.LongLength
                }
            };

            var uploadSession = await ErrorHandler.ExecuteWithErrorHandlingAsync(async () => await outlookClient.Me.Messages[request.MessageId].Attachments.CreateUploadSession
                .PostAsync(requestBody));

            using var memoryStream = new MemoryStream(fileBytes);
            var fileUploadTask = new LargeFileUploadTask<FileAttachment>(uploadSession, memoryStream, chunkSize);
            var uploadResult = await fileUploadTask.UploadAsync();
            var attachmentId = uploadResult.Location.Segments[^1].Split("'")[^2];
            attachment = await ErrorHandler.ExecuteWithErrorHandlingAsync(async () => await outlookClient.Me.Messages[request.MessageId].Attachments[attachmentId].GetAsync()) as FileAttachment;
        }

        return new FileAttachmentDto(attachment, fileManagementClient);
    }

    #endregion

    #region PATCH 

    [Action("Update draft message subject", Description = "Update the subject of a draft message.")]
    public async Task<MessageDto> UpdateDraftMessageSubject(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
        [ActionParameter] UpdateMessageSubjectRequest request)
    {
        var requestBody = new Message
        {
            Subject = request.Subject
        };
        var message = await ErrorHandler.ExecuteWithErrorHandlingAsync(async () => await outlookClient.Me.Messages[request.MessageId].PatchAsync(requestBody));
        var messageDto = new MessageDto(message);
        return messageDto;
    }

    [Action("Update draft message body", Description = "Update the body of a draft message. The body can be in " +
                                                             "html format or a plain string.")]
    public async Task<MessageDto> UpdateDraftMessageBody(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
        [ActionParameter] UpdateMessageBodyRequest request)
    {
        var requestBody = new Message
        {
            Body = new ItemBody { ContentType = BodyType.Html, Content = request.Content }
        };
        var message = await ErrorHandler.ExecuteWithErrorHandlingAsync(async () => await outlookClient.Me.Messages[request.MessageId].PatchAsync(requestBody));
        var messageDto = new MessageDto(message);
        return messageDto;
    }

    [Action("Add recipients to draft message", Description = "Add one or more email recipients to an existing " +
                                                                   "recipients list of a draft message.")]
    public async Task<MessageDto> AddRecipientsToDraftMessage(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
        [ActionParameter] RecipientEmailsRequest request)
    {
        var existingMessage = await outlookClient.Me.Messages[request.MessageId].GetAsync();
        var messageRecipients = existingMessage.ToRecipients ?? new List<Recipient>();
        messageRecipients.AddRange(
            request.RecipientEmails.Select(email => new Recipient { EmailAddress = new EmailAddress { Address = email } }));
        var requestBody = new Message
        {
            ToRecipients = messageRecipients
        };
        var message = await ErrorHandler.ExecuteWithErrorHandlingAsync(async () => await outlookClient.Me.Messages[request.MessageId].PatchAsync(requestBody));
        var messageDto = new MessageDto(message);
        return messageDto;
    }

    [Action("Remove recipients from draft message", Description = "Remove one or more email recipients from an " +
                                                                        "existing recipients list of a draft message.")]
    public async Task<MessageDto> RemoveEmailRecipients(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
        [ActionParameter] RecipientEmailsRequest request)
    {
        var existingMessage = await ErrorHandler.ExecuteWithErrorHandlingAsync(async () => await outlookClient.Me.Messages[request.MessageId].GetAsync());
        var messageRecipients = existingMessage.ToRecipients ?? new List<Recipient>();
        if (messageRecipients.Count > 0)
        {
            foreach (var email in request.RecipientEmails)
            {
                var index = messageRecipients.FindIndex(recipient => recipient.EmailAddress.Address == email);
                if (index != -1)
                    messageRecipients.RemoveAt(index);
            }
        }
        var requestBody = new Message
        {
            ToRecipients = messageRecipients
        };
        var message = await ErrorHandler.ExecuteWithErrorHandlingAsync(async () => await outlookClient.Me.Messages[request.MessageId].PatchAsync(requestBody));
        var messageDto = new MessageDto(message);
        return messageDto;
    }

    #endregion

    #region DELETE

    [Action("Delete message", Description = "Delete a message. The message can be either sent or a draft.")]
    public async Task DeleteMessage(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
        [ActionParameter] DeleteMessageRequest request)
    {
        await ErrorHandler.ExecuteWithErrorHandlingAsync(async () => await outlookClient.Me.Messages[request.MessageId].DeleteAsync());
    }

    #endregion
}