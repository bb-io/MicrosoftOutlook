using Blackbird.Applications.Sdk.Common;

namespace Apps.MicrosoftOutlook.Models.Contact.Requests;

public class BusinessPhoneRequest
{
    [Display("Contact ID")]
    public string ContactId { get; set; }
    
    [Display("Business phone number")]
    public string BusinessPhone { get; set; }
}