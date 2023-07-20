using Blackbird.Applications.Sdk.Common;

namespace Apps.MicrosoftOutlook.Models.Event.Requests;

public class GetEventRequest
{
    [Display("Event ID")]
    public string EventId { get; set; }
}