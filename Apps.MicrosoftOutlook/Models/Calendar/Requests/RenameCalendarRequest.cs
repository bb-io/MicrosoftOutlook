using Blackbird.Applications.Sdk.Common;

namespace Apps.MicrosoftOutlook.Models.Calendar.Requests;

public class RenameCalendarRequest
{
    [Display("Calendar ID")]
    public string? CalendarId { get; set; }
    
    [Display("New calendar name")]
    public string CalendarName { get; set; }
}