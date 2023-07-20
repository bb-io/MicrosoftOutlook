using Blackbird.Applications.Sdk.Common;

namespace Apps.MicrosoftOutlook.Models.Event.Requests;

public class CancelEventRequest
{
    [Display("Event or event occurrence ID")]
    public string EventOrEventOccurrenceId { get; set; }

    public string? Comment { get; set; }
}