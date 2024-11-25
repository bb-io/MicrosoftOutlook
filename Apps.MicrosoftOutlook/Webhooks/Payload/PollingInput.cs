using Apps.MicrosoftOutlook.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.MicrosoftOutlook.Webhooks.Payload;
public class PollingInput
{
    [Display("Sender email")]
    public string? Email { get; set; }

    [Display("Receiver email")]
    public string? ReceiverEmail { get; set; }

    [Display("Message folder")]
    [DataSource(typeof(MailFolderDataSourceHandler))]
    public string? MailFolderId { get; set; }
}

