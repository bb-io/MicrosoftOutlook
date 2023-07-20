using Blackbird.Applications.Sdk.Common;

namespace Apps.MicrosoftOutlook.Models.Calendar.Requests;

public class GetCalendarRequest
{
    [Display("Calendar ID")]
    public string? CalendarId { get; set; }
}