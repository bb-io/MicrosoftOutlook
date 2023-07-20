using Blackbird.Applications.Sdk.Common;

namespace Apps.MicrosoftOutlook.Models.Contact.Requests;

public class DeleteContactRequest
{
    [Display("Contact ID")]
    public string ContactId { get; set; }
}