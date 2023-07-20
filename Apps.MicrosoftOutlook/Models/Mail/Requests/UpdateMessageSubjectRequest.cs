using Blackbird.Applications.Sdk.Common;

namespace Apps.MicrosoftOutlook.Models.Mail.Requests;

public class UpdateMessageSubjectRequest
{
    [Display("Message ID")]
    public string MessageId { get; set; }
    
    public string Subject { get; set; }
}