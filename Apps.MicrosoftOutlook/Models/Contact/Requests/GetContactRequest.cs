using Apps.MicrosoftOutlook.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.MicrosoftOutlook.Models.Contact.Requests;

public class GetContactRequest
{
    [Display("Contact")]
    [DataSource(typeof(ContactDataSourceHandler))]
    public string ContactId { get; set; }
}