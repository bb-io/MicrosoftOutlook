using Blackbird.Applications.Sdk.Common;

namespace Apps.MicrosoftOutlook.Models.Event.Requests;

public class ListEventOccurrencesRequest
{
    [Display("Event ID")]
    public string EventId { get; set; }
    
    [Display("From")]
    public DateTime StartDate { get; set; }
    
    [Display("Till")]
    public DateTime EndDate { get; set; }
}