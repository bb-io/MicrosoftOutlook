using Apps.MicrosoftOutlook.Dtos;
using Apps.MicrosoftOutlook.Webhooks.Inputs;
using Apps.MicrosoftOutlook.Webhooks.Payload;
using Blackbird.Applications.Sdk.Common.Authentication;

namespace Apps.MicrosoftOutlook.Webhooks.Lists.ItemGetters;

public class MessageWithSenderGetter : ItemGetter<MessageDto>
{
    private readonly SenderInput _sender;

    public MessageWithSenderGetter(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
        SenderInput sender) : base(authenticationCredentialsProviders)
    {
        _sender = sender;
    }

    public override async Task<MessageDto?> GetItem(EventPayload eventPayload)
    {
        var client = new MicrosoftOutlookClient(AuthenticationCredentialsProviders);
        var message = await client.Me.Messages[eventPayload.ResourceData.Id].GetAsync();

        if (_sender.Email is not null && message.Sender.EmailAddress.Address != _sender.Email)
            return null;
        
        return new MessageDto(message);
    }
}