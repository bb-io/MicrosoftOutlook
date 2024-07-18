using Apps.MicrosoftOutlook.Webhooks.Inputs;
using Blackbird.Applications.Sdk.Common.Webhooks;

namespace Apps.MicrosoftOutlook.Webhooks.Handlers.Mail;

public class MessageReceivedWebhookHandler : BaseWebhookHandler
{
    private const string SubscriptionEvent = "created";
    
    public MessageReceivedWebhookHandler([WebhookParameter(true)] IWebhookInput input) : base(input, SubscriptionEvent) { }

    protected override string GetResource() => "/me/mailFolders/inbox/messages";
}