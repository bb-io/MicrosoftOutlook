using Blackbird.Applications.Sdk.Common;

namespace Apps.MicrosoftOutlook.Webhooks.Inputs;

public class ReceiverInput
{
    [Display("Receiver email")]
    public string? Email { get; set; }
}
