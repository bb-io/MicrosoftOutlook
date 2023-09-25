using Apps.MicrosoftOutlook.Dtos;
using Apps.MicrosoftOutlook.Webhooks.Payload;
using Blackbird.Applications.Sdk.Common.Authentication;

namespace Apps.MicrosoftOutlook.Webhooks.Lists.ItemGetters;

public class MessageGetter : ItemGetter<MessageDto>
{
    public MessageGetter(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders) 
        : base(authenticationCredentialsProviders) { }

    public override async Task<MessageDto?> GetItem(EventPayload eventPayload)
    {
        var client = new MicrosoftOutlookClient(AuthenticationCredentialsProviders);
        var message = await client.Me.Messages[eventPayload.ResourceData.Id].GetAsync();
        return new MessageDto(message);
    }
}