using Blackbird.Applications.Sdk.Common;

namespace Apps.MicrosoftOutlook.Models.Event.Requests;

public class UpdateEventRequest
{
    public string? Subject { get; set; }
    
    public string? Location { get; set; }
    
    [Display("Body content")]
    public string? BodyContent { get; set; }

    [Display("Date event takes place")]
    public DateTime? EventDate { get; set; }
    
    [Display("Start time in hh:mm format")]
    public string? StartTime { get; set; }
    
    [Display("End time in hh:mm format")]
    public string? EndTime { get; set; }

    [Display("Is online meeting")] 
    public bool? IsOnlineMeeting { get; set; }
    
    [Display("Is reminder on")]
    public bool? IsReminderOn { get; set; }

    [Display("Minutes till event that reminder alert occurs")]
    public int? ReminderMinutesBeforeStart { get; set; }

    [Display("Attendee emails")]
    public IEnumerable<string>? AttendeeEmails { get; set; }
}