using Blackbird.Applications.Sdk.Common;

namespace Apps.MicrosoftOutlook.Models.Mail.Requests;

public class GetMessageRequest
{
    [Display("Message ID")]
    public string MessageId { get; set; }
}