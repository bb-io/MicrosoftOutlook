using Apps.MicrosoftOutlook.Dtos;

namespace Apps.MicrosoftOutlook.Models.Calendar.Responses;

public class GetScheduleResponse
{
    public IEnumerable<ScheduleDto> Schedules { get; set; }
}