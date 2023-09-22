using Apps.MicrosoftOutlook.Webhooks.Inputs;
using Blackbird.Applications.Sdk.Common.Webhooks;

namespace Apps.MicrosoftOutlook.Webhooks.Handlers.Mail;

public class MessageCreatedWebhookHandler : MessageBaseWebhookHandler
{
    private const string SubscriptionEvent = "created";
    
    public MessageCreatedWebhookHandler([WebhookParameter(true)] MailFolderInput input) 
        : base(input, SubscriptionEvent) { }
}