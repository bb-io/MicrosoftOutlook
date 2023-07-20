using System.Globalization;
using Blackbird.Applications.Sdk.Common;
using Microsoft.Graph.Models;

namespace Apps.MicrosoftOutlook.Dtos;

public class EventDto
{
    public EventDto(Event calendarEvent)
    {
        EventId = calendarEvent.Id;
        Subject = calendarEvent.Subject;
        Link = calendarEvent.WebLink;
        EventDate = calendarEvent.Start.ToDateTime().Date;
        StartTime = calendarEvent.Start.ToDateTime().ToLocalTime().ToLongTimeString();
        EndTime = calendarEvent.End.ToDateTime().ToLocalTime().ToLongTimeString();
        Location = calendarEvent.Location.DisplayName;
        Attendees = calendarEvent.Attendees.Select(a => new AttendeeDto
        {
            Name = a.EmailAddress.Name, 
            Email = a.EmailAddress.Address, 
            Type = a.Type.ToString()
        });
        JoinUrl = calendarEvent.OnlineMeeting?.JoinUrl ?? "Offline meeting";
        BodyPreview = calendarEvent.BodyPreview;
        IsReminderOn = calendarEvent.IsReminderOn;
        ReminderMinutesBeforeStart = calendarEvent.ReminderMinutesBeforeStart;
        CreatedDateTime = calendarEvent.CreatedDateTime.Value.LocalDateTime;
        LastModifiedDateTime = calendarEvent.LastModifiedDateTime.Value.LocalDateTime;
        Recurrence = calendarEvent.Recurrence == null
            ? null
            : new RecurrenceDto
            {
                RecurrencePattern = calendarEvent.Recurrence.Pattern.Type.Value.ToString(),
                Interval = calendarEvent.Recurrence.Pattern.Interval.Value,
                DaysOfWeek = calendarEvent.Recurrence.Pattern.DaysOfWeek?.Select(d => d.ToString()),
                RecurrenceEndDateTime = calendarEvent.Recurrence.Range.EndDate.Value.DateTime
            };
    }
    
    [Display("Event ID")]
    public string EventId { get; set; }
    
    public string Subject { get; set; }
    
    public string Link { get; set; }
    
    public string Location { get; set; }
    
    public IEnumerable<AttendeeDto> Attendees { get; set; }
    
    [Display("Date event takes place")]
    public DateTime EventDate { get; set; }
    
    [Display("Start time")]
    public string StartTime { get; set; }
    
    [Display("End time")]
    public string EndTime { get; set; }

    [Display("Join url")]
    public string? JoinUrl { get; set; }
    
    [Display("Body preview")]
    public string? BodyPreview { get; set; }
    
    [Display("Is reminder on")]
    public bool? IsReminderOn { get; set; }
    
    [Display("Minutes till event that reminder alert occurs")]
    public int? ReminderMinutesBeforeStart { get; set; }
    
    [Display("Created")]
    public DateTime CreatedDateTime { get; set; }

    [Display("Last modified")]
    public DateTime LastModifiedDateTime { get; set; }
    
    public RecurrenceDto? Recurrence { get; set; }
}

public class AttendeeDto
{
    public string Email { get; set; }
    
    public string? Name { get; set; }
    
    public string Type { get; set; }
}

public class RecurrenceDto
{
    [Display("Recurrence pattern")]
    public string RecurrencePattern { get; set; }

    [Display("Recurrence interval")] 
    public int Interval { get; set; } = 1; 
    
    [Display("Days of week recurrence pattern applies to")]
    public IEnumerable<string>? DaysOfWeek { get; set; }

    [Display("Event recurrence end date")] 
    public DateTime RecurrenceEndDateTime { get; set; }
}