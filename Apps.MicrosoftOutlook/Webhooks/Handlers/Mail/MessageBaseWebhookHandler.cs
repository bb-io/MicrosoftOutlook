using Apps.MicrosoftOutlook.Webhooks.Inputs;
using Blackbird.Applications.Sdk.Common.Webhooks;

namespace Apps.MicrosoftOutlook.Webhooks.Handlers.Mail;

public class MessageBaseWebhookHandler : BaseWebhookHandler
{
    protected MessageBaseWebhookHandler([WebhookParameter(true)] MailFolderInput input, string subscriptionEvent)
        : base(input, subscriptionEvent) { }
    
    protected override string GetResource()
    {
        var mailFolderInput = (MailFolderInput)webhookInput;
        var resource = $"/me/mailFolders/{mailFolderInput.MailFolderId}/messages";
        return resource;
    }
}