using Apps.MicrosoftOutlook.Dtos;

namespace Apps.MicrosoftOutlook.Models.Mail.Responses;

public class ListMailFoldersResponse
{
    public IEnumerable<MailFolderDto> MailFolders { get; set; }
}