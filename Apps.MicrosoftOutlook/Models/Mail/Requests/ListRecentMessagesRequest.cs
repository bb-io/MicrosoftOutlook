using Apps.MicrosoftOutlook.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.SDK.Extensions.FileManagement.Models.FileDataSourceItems;

namespace Apps.MicrosoftOutlook.Models.Mail.Requests;

public class ListRecentMessagesRequest
{
    public int? Hours { get; set; }
    
    [Display("Message folder")]
    [FileDataSource(typeof(MailFolderDataSourceHandler))]
    public string? MailFolderId { get; set; }
}