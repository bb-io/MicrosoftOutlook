using Apps.MicrosoftOutlook.Webhooks.Inputs;
using Blackbird.Applications.Sdk.Common.Webhooks;

namespace Apps.MicrosoftOutlook.Webhooks.Handlers.Mail;

public class MessageUpdatedWebhookHandler : MessageBaseWebhookHandler
{
    private const string SubscriptionEvent = "updated";
    
    public MessageUpdatedWebhookHandler([WebhookParameter(true)] MailFolderInput input) 
        : base(input, SubscriptionEvent) { }
}