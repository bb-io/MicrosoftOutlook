using Apps.MicrosoftOutlook.Dtos;
using Apps.MicrosoftOutlook.Webhooks.Handlers.Mail;
using Apps.MicrosoftOutlook.Webhooks.Inputs;
using Apps.MicrosoftOutlook.Webhooks.Lists.ItemGetters;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common.Webhooks;

namespace Apps.MicrosoftOutlook.Webhooks.Lists;

[WebhookList]
public class MailWebhooks : BaseWebhookList
{
    public MailWebhooks(InvocationContext invocationContext) : base(invocationContext) { }

    [Webhook("On message created", typeof(MessageCreatedWebhookHandler), 
        Description = "This webhook is triggered when a new message is created.")]
    public async Task<WebhookResponse<MessageDto>> OnMessageCreated(WebhookRequest request)
    {
        return await HandleWebhookRequest(request, new MessageGetter(AuthenticationCredentialsProviders));
    }
    
    [Webhook("On message updated", typeof(MessageUpdatedWebhookHandler), 
        Description = "This webhook is triggered when a message is updated.")]
    public async Task<WebhookResponse<MessageDto>> OnMessageUpdated(WebhookRequest request)
    {
        return await HandleWebhookRequest(request, new MessageGetter(AuthenticationCredentialsProviders));
    }
    
    [Webhook("On message received", typeof(MessageReceivedWebhookHandler), 
        Description = "This webhook is triggered when a new message is received.")]
    public async Task<WebhookResponse<MessageDto>> OnMessageReceived(WebhookRequest request, 
        [WebhookParameter] SenderInput sender)
    {
        return await HandleWebhookRequest(request, 
            new MessageWithSenderGetter(AuthenticationCredentialsProviders, sender));
    }
    
    [Webhook("On message with files attached received", typeof(MessageReceivedWebhookHandler), 
        Description = "This webhook is triggered when a message with file attachments is received.")]
    public async Task<WebhookResponse<MessageDto>> OnMessageWithAttachmentsReceived(WebhookRequest request, 
        [WebhookParameter] SenderInput sender)
    {
        return await HandleWebhookRequest(request, 
            new MessageWithSenderAndAttachmentsGetter(AuthenticationCredentialsProviders, sender));
    }
}