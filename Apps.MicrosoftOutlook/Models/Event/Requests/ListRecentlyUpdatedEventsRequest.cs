using Apps.MicrosoftOutlook.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.MicrosoftOutlook.Models.Event.Requests;

public class ListRecentlyUpdatedEventsRequest
{
    [Display("Calendar")]
    [DataSource(typeof(CalendarDataSourceHandler))]
    public string? CalendarId { get; set; }

    public int? Hours { get; set; }
}