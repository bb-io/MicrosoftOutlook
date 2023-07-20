using Blackbird.Applications.Sdk.Common;
using Microsoft.Graph.Models;

namespace Apps.MicrosoftOutlook.Dtos;

public class ScheduleDto
{
    public ScheduleDto(ScheduleInformation schedule)
    {
        Email = schedule.ScheduleId;
        ScheduleItems = schedule.ScheduleItems.Select(s => new ScheduleItemDto
        {
            Status = s.Status.ToString(),
            Subject = s.Subject,
            Location = s.Location,
            StartDateTime = s.Start.ToDateTime().ToLocalTime(),
            EndDateTime = s.End.ToDateTime().ToLocalTime()
        });
    }
    
    public string Email { get; set; }

    public IEnumerable<ScheduleItemDto> ScheduleItems { get; set; }
}

public class ScheduleItemDto
{
    public string Status { get; set; }
    
    public string? Subject { get; set; }
    
    public string? Location { get; set; }
    
    [Display("From")]
    public DateTime StartDateTime { get; set; }
    
    [Display("Till")]
    public DateTime EndDateTime { get; set; }
}