using Blackbird.Applications.Sdk.Common;
using Microsoft.Graph.Models;

namespace Apps.MicrosoftOutlook.Dtos;

public class FileAttachmentDto
{
    public FileAttachmentDto(FileAttachment attachment)
    {
        AttachmentId = attachment.Id;
        File = attachment.ContentBytes;
        Filename = attachment.Name;
        ContentType = attachment.ContentType;
        SizeBytes = attachment.Size.Value;
    }
    
    [Display("Attachment ID")]
    public string AttachmentId { get; set; }
    
    public byte[] File { get; set; }
    
    public string Filename { get; set; }
    
    [Display("Content type")]
    public string ContentType { get; set; }
    
    [Display("Size in bytes")]
    public int SizeBytes { get; set; }
}