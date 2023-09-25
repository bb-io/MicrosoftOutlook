using Blackbird.Applications.Sdk.Common;

namespace Apps.MicrosoftOutlook.Webhooks.Inputs;

public class SenderInput
{
    [Display("Sender email")]
    public string? Email { get; set; }
}