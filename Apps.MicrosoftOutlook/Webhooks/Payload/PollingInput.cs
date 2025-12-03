using Apps.MicrosoftOutlook.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.SDK.Extensions.FileManagement.Models.FileDataSourceItems;

namespace Apps.MicrosoftOutlook.Webhooks.Payload;

public class PollingInput
{
    [Display("Sender email")]
    public string? Email { get; set; }

    [Display("Receiver email")]
    public string? ReceiverEmail { get; set; }

    [Display("Message folder")]
    [FileDataSource(typeof(MailFolderDataSourceHandler))]
    public string? MailFolderId { get; set; }

    [Display("Content contains")]
    public string? ContentContains { get; set; }

    [Display("Subject contains")]
    public string? SubjectContains { get; set; }
}