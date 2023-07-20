using Apps.MicrosoftOutlook.Dtos;

namespace Apps.MicrosoftOutlook.Models.Contact.Responses;

public class ListContactsResponse
{
    public IEnumerable<ContactDto> Contacts { get; set; }
}