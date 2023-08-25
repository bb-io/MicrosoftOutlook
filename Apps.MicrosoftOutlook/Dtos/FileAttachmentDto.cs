using Blackbird.Applications.Sdk.Common;
using Microsoft.Graph.Models;
using File = Blackbird.Applications.Sdk.Common.Files.File;

namespace Apps.MicrosoftOutlook.Dtos;

public class FileAttachmentDto
{
    public FileAttachmentDto(FileAttachment attachment)
    {
        AttachmentId = attachment.Id;
        File = new File(attachment.ContentBytes)
        {
            Name = attachment.Name, 
            ContentType = attachment.ContentType
        };
    }
    
    [Display("Attachment ID")]
    public string AttachmentId { get; set; }
    
    public File File { get; set; }
}