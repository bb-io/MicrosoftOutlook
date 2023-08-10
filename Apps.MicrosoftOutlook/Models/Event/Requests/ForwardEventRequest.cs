using Apps.MicrosoftOutlook.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.MicrosoftOutlook.Models.Event.Requests;

public class ForwardEventRequest
{
    [Display("Event")]
    [DataSource(typeof(EventDataSourceHandler))]
    public string EventId { get; set; }
    
    [Display("Recipient email")]
    public string RecipientEmail { get; set; }

    [Display("Recipient name")] 
    public string? RecipientName { get; set; }

    public string? Comment { get; set; }
}