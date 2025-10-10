using Blackbird.Applications.Sdk.Common;

namespace Apps.MicrosoftOutlook.Webhooks.Inputs;

public class IWebhookInput
{
    [Display("Shared emails")]
    public List<string>? SharedEmails { get; set; }
}