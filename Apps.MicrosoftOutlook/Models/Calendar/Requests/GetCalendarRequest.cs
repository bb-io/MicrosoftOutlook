using Apps.MicrosoftOutlook.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.MicrosoftOutlook.Models.Calendar.Requests;

public class GetCalendarRequest
{
    [Display("Calendar")]
    [DataSource(typeof(CalendarDataSourceHandler))]
    public string? CalendarId { get; set; }
}