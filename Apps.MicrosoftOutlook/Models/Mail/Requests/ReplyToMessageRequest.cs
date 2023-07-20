using Blackbird.Applications.Sdk.Common;

namespace Apps.MicrosoftOutlook.Models.Mail.Requests;

public class ReplyToMessageRequest
{
    [Display("Message ID")]
    public string MessageId { get; set; }

    public string Comment { get; set; }
}