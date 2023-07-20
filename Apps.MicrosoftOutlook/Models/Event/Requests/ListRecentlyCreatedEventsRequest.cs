using Blackbird.Applications.Sdk.Common;

namespace Apps.MicrosoftOutlook.Models.Event.Requests;

public class ListRecentlyCreatedEventsRequest
{
    [Display("Calendar ID")]
    public string? CalendarId { get; set; }

    public int? Hours { get; set; }
}