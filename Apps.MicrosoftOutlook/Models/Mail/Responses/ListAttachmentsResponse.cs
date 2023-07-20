using Apps.MicrosoftOutlook.Dtos;

namespace Apps.MicrosoftOutlook.Models.Mail.Responses;

public class ListAttachmentsResponse
{
    public IEnumerable<FileAttachmentDto> Attachments { get; set; }
}