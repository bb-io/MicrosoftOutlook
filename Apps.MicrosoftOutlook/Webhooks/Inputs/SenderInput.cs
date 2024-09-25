using Blackbird.Applications.Sdk.Common;

namespace Apps.MicrosoftOutlook.Webhooks.Inputs;

public class SenderAndReceiverInput
{
    [Display("Sender email")]
    public string? Email { get; set; }

    [Display("Receiver email")]
    public string? ReceiverEmail { get; set; }
}