using Blackbird.Applications.Sdk.Common;

namespace Apps.MicrosoftOutlook.Models.Event.Requests;

public class ForwardEventRequest
{
    [Display("Event ID")]
    public string EventId { get; set; }
    
    [Display("Recipient email")]
    public string RecipientEmail { get; set; }

    [Display("Recipient name")] 
    public string? RecipientName { get; set; }

    public string? Comment { get; set; }
}