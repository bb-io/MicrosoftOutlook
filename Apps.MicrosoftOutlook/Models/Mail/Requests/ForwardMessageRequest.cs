using Blackbird.Applications.Sdk.Common;

namespace Apps.MicrosoftOutlook.Models.Mail.Requests;

public class ForwardMessageRequest
{
    [Display("Message ID")]
    public string MessageId { get; set; }
    
    [Display("Recipient emails")]
    public IEnumerable<string> RecipientEmails { get; set; }
    
    public string? Comment { get; set; }
}