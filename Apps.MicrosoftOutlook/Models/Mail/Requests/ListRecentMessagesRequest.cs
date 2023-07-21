using Blackbird.Applications.Sdk.Common;

namespace Apps.MicrosoftOutlook.Models.Mail.Requests;

public class ListRecentMessagesRequest
{
    public int? Hours { get; set; }
    
    [Display("Message folder ID")]
    public string? MailFolderId { get; set; }
}