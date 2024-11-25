using Blackbird.Applications.Sdk.Common;
using Microsoft.Graph.Models;
using System.Globalization;

namespace Apps.MicrosoftOutlook.Webhooks.Payload;
public class ReceivedMessageDto
{
    public ReceivedMessageDto(Message message)
    {
        MessageId = message.Id;
        Subject = message.Subject;
        Link = message.WebLink;
        SenderName = message.Sender?.EmailAddress?.Name;
        SenderEmail = message.Sender?.EmailAddress?.Address;
        Content = message.Body?.Content;
        IsDraft = message.IsDraft.Value;
        CreatedDateTime = message.CreatedDateTime?.ToLocalTime().ToString(CultureInfo.CurrentCulture);
        SentDateTime = message.SentDateTime?.DateTime ?? DateTime.MinValue;
        RecipientEmails = message.ToRecipients?.Select(r => r.EmailAddress.Address);
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
    public DateTime SentDateTime { get; set; }

    [Display("Recipient emails")]
    public IEnumerable<string> RecipientEmails { get; set; }
}

