using Apps.MicrosoftOutlook.Dtos;

namespace Apps.MicrosoftOutlook.Models.Event.Responses;

public class ListEventsResponse
{
    public IEnumerable<EventDto> Events { get; set; }
}