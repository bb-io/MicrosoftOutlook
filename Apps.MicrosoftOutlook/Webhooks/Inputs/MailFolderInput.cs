using Apps.MicrosoftOutlook.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.SDK.Extensions.FileManagement.Models.FileDataSourceItems;

namespace Apps.MicrosoftOutlook.Webhooks.Inputs;

public class MailFolderInput : IWebhookInput
{
    [Display("Message folder")]
    [FileDataSource(typeof(MailFolderDataSourceHandler))]
    public string MailFolderId { get; set; }
}