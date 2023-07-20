using Blackbird.Applications.Sdk.Common;

namespace Apps.MicrosoftOutlook.Models.Mail.Requests;

public class RecipientEmailsRequest
{
    [Display("Message ID")]
    public string MessageId { get; set; }
    
    [Display("Recipient emails")]
    public IEnumerable<string> RecipientEmails { get; set; }
}