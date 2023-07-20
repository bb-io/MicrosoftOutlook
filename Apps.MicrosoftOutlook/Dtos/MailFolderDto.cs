using Blackbird.Applications.Sdk.Common;
using Microsoft.Graph.Models;

namespace Apps.MicrosoftOutlook.Dtos;

public class MailFolderDto
{
    public MailFolderDto(MailFolder mailFolder)
    {
        MailFolderId = mailFolder.Id;
        Name = mailFolder.DisplayName;
        UnreadItemCount = mailFolder.UnreadItemCount.Value;
        TotalItemCount = mailFolder.TotalItemCount.Value;
    }
    
    [Display("Mail folder ID")]
    public string MailFolderId { get; set; }
    
    public string Name { get; set; }
    
    [Display("Unread item count")]
    public int UnreadItemCount { get; set; }
    
    [Display("Total item count")]
    public int TotalItemCount { get; set; }
}