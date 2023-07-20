using Blackbird.Applications.Sdk.Common;

namespace Apps.MicrosoftOutlook.Models.Event.Requests;

public class ListEventsRequest
{
    [Display("Calendar ID")]
    public string? CalendarId { get; set; }
}