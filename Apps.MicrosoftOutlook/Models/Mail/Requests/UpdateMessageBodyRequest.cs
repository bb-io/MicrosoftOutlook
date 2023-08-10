using Apps.MicrosoftOutlook.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.MicrosoftOutlook.Models.Mail.Requests;

public class UpdateMessageBodyRequest
{
    [Display("Message")]
    [DataSource(typeof(DraftMessageDataSourceHandler))]
    public string MessageId { get; set; }
    
    public string Content { get; set; }
}