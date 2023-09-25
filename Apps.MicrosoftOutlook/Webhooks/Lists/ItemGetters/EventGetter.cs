using Apps.MicrosoftOutlook.Dtos;
using Apps.MicrosoftOutlook.Webhooks.Payload;
using Blackbird.Applications.Sdk.Common.Authentication;

namespace Apps.MicrosoftOutlook.Webhooks.Lists.ItemGetters;

public class EventGetter : ItemGetter<EventDto>
{
    public EventGetter(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders) 
        : base(authenticationCredentialsProviders) { }

    public override async Task<EventDto?> GetItem(EventPayload eventPayload)
    {
        var client = new MicrosoftOutlookClient(AuthenticationCredentialsProviders);
        var calendarEvent = await client.Me.Events[eventPayload.ResourceData.Id].GetAsync();
        return new EventDto(calendarEvent);
    }
}