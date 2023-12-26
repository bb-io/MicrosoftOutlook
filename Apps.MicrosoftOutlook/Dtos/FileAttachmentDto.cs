using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Files;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using Microsoft.Graph.Models;

namespace Apps.MicrosoftOutlook.Dtos;

public class FileAttachmentDto
{
    public FileAttachmentDto(FileAttachment attachment, IFileManagementClient fileManagementClient)
    {
        using var stream = new MemoryStream(attachment.ContentBytes);
        AttachmentId = attachment.Id;
        File = fileManagementClient.UploadAsync(stream, attachment.ContentType, attachment.Name).Result;
    }
    
    [Display("Attachment ID")]
    public string AttachmentId { get; set; }
    
    public FileReference File { get; set; }
}