using Apps.MicrosoftOutlook.Dtos;
using Apps.MicrosoftOutlook.Webhooks.Handlers.Mail;
using Apps.MicrosoftOutlook.Webhooks.Inputs;
using Apps.MicrosoftOutlook.Webhooks.Lists.ItemGetters;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common.Webhooks;

namespace Apps.MicrosoftOutlook.Webhooks.Lists;

[WebhookList]
public class MailWebhooks(InvocationContext invocationContext) : BaseWebhookList(invocationContext)
{
    [Webhook("On email created", typeof(MessageCreatedWebhookHandler), 
        Description = "This webhook is triggered when a new email is created.")]
    public async Task<WebhookResponse<MessageDto>> OnEmailCreated(WebhookRequest request)
    {
        return await HandleWebhookRequest(request, new MessageGetter(AuthenticationCredentialsProviders));
    }
    
    [Webhook("On email updated", typeof(MessageUpdatedWebhookHandler), 
        Description = "This webhook is triggered when an email is updated.")]
    public async Task<WebhookResponse<MessageDto>> OnEmailUpdated(WebhookRequest request)
    {
        return await HandleWebhookRequest(request, new MessageGetter(AuthenticationCredentialsProviders));
    }
    
    [Webhook("On email received", typeof(MessageReceivedWebhookHandler), 
        Description = "This webhook is triggered when a new email is received.")]
    public async Task<WebhookResponse<MessageDto>> OnEmailReceived(WebhookRequest request, 
        [WebhookParameter] SenderInput sender)
    {
        return await HandleWebhookRequest(request, 
            new MessageWithSenderGetter(AuthenticationCredentialsProviders, sender));
    }
    
    [Webhook("On email with files attached received", typeof(MessageReceivedWebhookHandler), 
        Description = "This webhook is triggered when an email with file attachments is received.")]
    public async Task<WebhookResponse<MessageDto>> OnEmailWithAttachmentsReceived(WebhookRequest request, 
        [WebhookParameter] SenderInput sender)
    {
        return await HandleWebhookRequest(request, 
            new MessageWithSenderAndAttachmentsGetter(AuthenticationCredentialsProviders, sender));
    }
}