using Apps.MicrosoftOutlook.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.MicrosoftOutlook.Models.Calendar.Requests;

public class DeleteCalendarRequest
{
    [Display("Calendar")]
    [DataSource(typeof(NonDefaultCalendarDataSourceHandler))]
    public string CalendarId { get; set; }
}