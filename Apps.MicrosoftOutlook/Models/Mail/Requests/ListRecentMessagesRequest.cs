using Apps.MicrosoftOutlook.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.MicrosoftOutlook.Models.Mail.Requests;

public class ListRecentMessagesRequest
{
    public int? Hours { get; set; }
    
    [Display("Message folder")]
    [DataSource(typeof(MailFolderDataSourceHandler))]
    public string? MailFolderId { get; set; }
}