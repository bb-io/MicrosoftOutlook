using Apps.MicrosoftOutlook.Dtos;

namespace Apps.MicrosoftOutlook.Models.Mail.Responses;

public class ListRecentMessagesResponse
{
    public IEnumerable<MessageDto> Messages { get; set; }
}