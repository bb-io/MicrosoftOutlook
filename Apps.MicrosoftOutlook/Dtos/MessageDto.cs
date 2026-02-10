using System.Globalization;
using Blackbird.Applications.Sdk.Common;
using Microsoft.Graph.Models;

namespace Apps.MicrosoftOutlook.Dtos;

public class MessageDto
{
    public MessageDto(Message message)
    {
        MessageId = message.Id;
        Subject = message.Subject;
        Link = message.WebLink;
        SenderName = message.Sender?.EmailAddress?.Name;
        SenderEmail = message.Sender?.EmailAddress?.Address;
        Content = message.Body?.Content;
        IsDraft = message.IsDraft.Value;
        CreatedDateTime = message.CreatedDateTime?.ToLocalTime().ToString(CultureInfo.CurrentCulture); 
        SentDateTime = message.SentDateTime?.ToLocalTime().ToString(CultureInfo.CurrentCulture);
        RecipientEmails = message.ToRecipients?.Select(r => r.EmailAddress.Address);
        CcRecipientEmails = message.CcRecipients?
           .Select(r => r.EmailAddress?.Address)
           .Where(x => !string.IsNullOrWhiteSpace(x))
           .Distinct()
           .ToList()
           ?? new List<string>();

        BccRecipientEmails = message.BccRecipients?
            .Select(r => r.EmailAddress?.Address)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Distinct()
            .ToList()
            ?? new List<string>();
    }

    [Display("Message ID")]
    public string MessageId { get; set; }
    
    public string? Subject { get; set; }
    
    public string Link { get; set; }
    
    public string? Content { get; set; }
    
    [Display("Is draft")]
    public bool IsDraft { get; set; }
    
    [Display("Sender name")]
    public string? SenderName { get; set; }
    
    [Display("Sender email")]
    public string? SenderEmail { get; set; }

    [Display("Date created")]
    public string CreatedDateTime { get; set; }
    
    [Display("Date sent")]
    public string? SentDateTime { get; set; }
    
    [Display("Recipient emails")]
    public IEnumerable<string> RecipientEmails { get; set; }

    [Display("CC recipient emails")]
    public IEnumerable<string> CcRecipientEmails { get; set; } = new List<string>();

    [Display("BCC recipient emails")]
    public IEnumerable<string> BccRecipientEmails { get; set; } = new List<string>();
}