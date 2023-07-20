using Blackbird.Applications.Sdk.Common;

namespace Apps.MicrosoftOutlook.Models.Calendar.Requests;

public class CreateCalendarRequest
{
    [Display("Calendar name")]
    public string CalendarName { get; set; }
}