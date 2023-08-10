using Apps.MicrosoftOutlook.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.MicrosoftOutlook.Models.Mail.Requests;

public class UpdateMessageSubjectRequest
{
    [Display("Message")]
    [DataSource(typeof(DraftMessageDataSourceHandler))]
    public string MessageId { get; set; }
    
    public string Subject { get; set; }
}