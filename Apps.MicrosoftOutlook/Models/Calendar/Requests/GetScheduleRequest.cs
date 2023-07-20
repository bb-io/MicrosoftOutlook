using Blackbird.Applications.Sdk.Common;

namespace Apps.MicrosoftOutlook.Models.Calendar.Requests;

public class GetScheduleRequest
{
    [Display("Users' emails")]
    public List<string> Emails { get; set; }
    
    [Display("From")]
    public DateTime StartDateTime { get; set; }
    
    [Display("Till")]
    public DateTime EndDateTime { get; set; }
}