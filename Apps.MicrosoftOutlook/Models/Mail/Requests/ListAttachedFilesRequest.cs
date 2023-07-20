using Blackbird.Applications.Sdk.Common;

namespace Apps.MicrosoftOutlook.Models.Mail.Requests;

public class ListAttachedFilesRequest
{
    [Display("Message ID")]
    public string MessageId { get; set; }
}