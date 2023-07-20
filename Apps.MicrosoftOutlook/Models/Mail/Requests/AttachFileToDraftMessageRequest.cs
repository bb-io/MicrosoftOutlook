using Blackbird.Applications.Sdk.Common;

namespace Apps.MicrosoftOutlook.Models.Mail.Requests;

public class AttachFileToDraftMessageRequest
{
    [Display("Message ID")]
    public string MessageId { get; set; }
    
    public string Filename { get; set; }

    public byte[] File { get; set; }
}