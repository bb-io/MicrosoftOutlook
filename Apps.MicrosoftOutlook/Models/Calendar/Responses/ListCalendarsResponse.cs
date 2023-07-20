using Apps.MicrosoftOutlook.Dtos;

namespace Apps.MicrosoftOutlook.Models.Calendar.Responses;

public class ListCalendarsResponse
{
    public IEnumerable<CalendarDto> Calendars { get; set; }
}