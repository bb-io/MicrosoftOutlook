using Apps.MicrosoftOutlook.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.MicrosoftOutlook.Models.Event.Requests;

public class ListEventOccurrencesRequest
{
    [Display("Event")]
    [DataSource(typeof(EventDataSourceHandler))]
    public string EventId { get; set; }
    
    [Display("From")]
    public DateTime StartDate { get; set; }
    
    [Display("Till")]
    public DateTime EndDate { get; set; }
}