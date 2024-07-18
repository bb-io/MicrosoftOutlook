using Apps.MicrosoftOutlook.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.MicrosoftOutlook.Webhooks.Inputs;

public class IWebhookInput
{
    [Display("Contacts")]
    [DataSource(typeof(ContactDataSourceHandler))]
    public List<string> Contacts { get; set; }
}