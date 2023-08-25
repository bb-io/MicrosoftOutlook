using Apps.MicrosoftOutlook.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using File = Blackbird.Applications.Sdk.Common.Files.File;

namespace Apps.MicrosoftOutlook.Models.Mail.Requests;

public class AttachFileToDraftMessageRequest
{
    [Display("Message")]
    [DataSource(typeof(DraftMessageDataSourceHandler))]
    public string MessageId { get; set; }
    
    public File File { get; set; }
}