using Blackbird.Applications.Sdk.Common;

namespace Apps.MicrosoftOutlook.Models.Mail.Requests;

public class ListRecentMessagesRequest
{
    [Display("Messages amount")]
    public int? MessagesAmount { get; set; }
    
    [Display("Message folder id")]
    public string? MailFolderId { get; set; }
}