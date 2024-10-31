using Blackbird.Applications.Sdk.Common;

namespace Apps.MicrosoftOutlook.Models.Mail.Requests;

public class CreateMessageRequest
{
    public string Subject { get; set; }
    
    public string Content { get; set; }

    [Display("Recipient emails")]
    public IEnumerable<string> RecipientEmails { get; set; }

    [Display("Sender email", Description = "Use this property to send message from shared email")]
    public string? SenderEmail { get; set; }
}

