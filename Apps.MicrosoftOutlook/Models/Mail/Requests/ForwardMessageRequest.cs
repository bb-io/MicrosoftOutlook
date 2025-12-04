using Apps.MicrosoftOutlook.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.SDK.Extensions.FileManagement.Models.FileDataSourceItems;

namespace Apps.MicrosoftOutlook.Models.Mail.Requests;

public class ForwardMessageRequest
{
    [Display("Message ID")]
    [FileDataSource(typeof(MessageDataSourceHandler))]
    public string MessageId { get; set; }
    
    [Display("Recipient emails")]
    public IEnumerable<string> RecipientEmails { get; set; }
    
    public string? Comment { get; set; }
}