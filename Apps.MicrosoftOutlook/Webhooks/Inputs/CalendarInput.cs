using Apps.MicrosoftOutlook.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.MicrosoftOutlook.Webhooks.Inputs;

public class CalendarInput : IWebhookInput
{
    [Display("Calendar")]
    [DataSource(typeof(CalendarDataSourceHandler))]
    public string CalendarId { get; set; }
}