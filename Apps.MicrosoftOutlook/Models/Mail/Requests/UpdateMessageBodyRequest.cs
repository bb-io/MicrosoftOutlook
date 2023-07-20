using Blackbird.Applications.Sdk.Common;

namespace Apps.MicrosoftOutlook.Models.Mail.Requests;

public class UpdateMessageBodyRequest
{
    [Display("Message ID")]
    public string MessageId { get; set; }
    
    public string Content { get; set; }
}