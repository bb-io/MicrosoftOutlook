using Apps.MicrosoftOutlook.Webhooks.Inputs;
using Blackbird.Applications.Sdk.Common.Webhooks;

namespace Apps.MicrosoftOutlook.Webhooks.Handlers.Mail;

public class MessageCreatedWebhookHandler : BaseWebhookHandler
{
    private const string SubscriptionEvent = "created";
    
    public MessageCreatedWebhookHandler([WebhookParameter(true)] MailFolderInput input) 
        : base(input, SubscriptionEvent) { }
    
    protected override string GetResource()
    {
        var mailFolderInput = (MailFolderInput)WebhookInput;
        var resource = $"/me/mailFolders/{mailFolderInput.MailFolderId}/messages";
        return resource;
    }
}