namespace Apps.MicrosoftOutlook.Webhooks.Handlers.Mail;

public class MessageReceivedWebhookHandler : BaseWebhookHandler
{
    private const string SubscriptionEvent = "created";
    
    public MessageReceivedWebhookHandler() : base(SubscriptionEvent) { }

    protected override string GetResource() => "/me/mailFolders/inbox/messages";
}