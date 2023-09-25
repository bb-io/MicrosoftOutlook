using Apps.MicrosoftOutlook.Dtos;
using Apps.MicrosoftOutlook.Webhooks.Handlers.Mail;
using Apps.MicrosoftOutlook.Webhooks.Payload;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common.Webhooks;
using Microsoft.Graph.Models;

namespace Apps.MicrosoftOutlook.Webhooks.Lists;

[WebhookList]
public class MailWebhooks : BaseWebhookList
{
    public MailWebhooks(InvocationContext invocationContext) : base(invocationContext) { }

    [Webhook("On message created", typeof(MessageCreatedWebhookHandler), 
        Description = "This webhook is triggered when a new message is created.")]
    public async Task<WebhookResponse<MessageDto>> OnMessageCreated(WebhookRequest request)
    {
        return await HandleWebhookRequest(request, GetMessage);
    }
    
    [Webhook("On message updated", typeof(MessageUpdatedWebhookHandler), 
        Description = "This webhook is triggered when a message is updated.")]
    public async Task<WebhookResponse<MessageDto>> OnMessageUpdated(WebhookRequest request)
    {
        return await HandleWebhookRequest(request, GetMessage);
    }
    
    [Webhook("On message with files attached created", typeof(MessageCreatedWebhookHandler), 
        Description = "This webhook is triggered when a message with file attachments is created.")]
    public async Task<WebhookResponse<MessageDto>> OnMessageWithAttachmentsCreated(WebhookRequest request)
    {
        return await HandleWebhookRequest(request, GetMessageWithAttachments);
    }

    private async Task<MessageDto?> GetMessage(EventPayload eventPayload)
    {
        var client = new MicrosoftOutlookClient(InvocationContext.AuthenticationCredentialsProviders);
        var message = await client.Me.Messages[eventPayload.ResourceData.Id].GetAsync();
        return new MessageDto(message);
    }

    private async Task<MessageDto?> GetMessageWithAttachments(EventPayload eventPayload)
    {
        var client = new MicrosoftOutlookClient(InvocationContext.AuthenticationCredentialsProviders);
        var attachments = await client.Me.Messages[eventPayload.ResourceData.Id].Attachments.GetAsync();
        var fileAttachments = attachments.Value.Where(a => a is FileAttachment);

        if (!fileAttachments.Any())
            return null;
        
        var message = await client.Me.Messages[eventPayload.ResourceData.Id].GetAsync();
        return new MessageDto(message);
    } 
}