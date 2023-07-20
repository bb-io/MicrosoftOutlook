using Blackbird.Applications.Sdk.Common;

namespace Apps.MicrosoftOutlook.Models.Contact.Requests;

public class EmailRequest
{
    [Display("Contact ID")]
    public string ContactId { get; set; }
    
    public string Email { get; set; }
}