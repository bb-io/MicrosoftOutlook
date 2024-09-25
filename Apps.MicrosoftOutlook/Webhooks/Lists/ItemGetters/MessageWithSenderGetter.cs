using Apps.MicrosoftOutlook.Dtos;
using Apps.MicrosoftOutlook.Webhooks.Inputs;
using Apps.MicrosoftOutlook.Webhooks.Payload;
using Blackbird.Applications.Sdk.Common.Authentication;

namespace Apps.MicrosoftOutlook.Webhooks.Lists.ItemGetters;

public class MessageWithSenderGetter : ItemGetter<MessageDto>
{
    private readonly SenderInput _sender;
    private readonly ReceiverInput _receiver;

    public MessageWithSenderGetter(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
        SenderInput sender, ReceiverInput receiver) : base(authenticationCredentialsProviders)
    {
        _sender = sender;
        _receiver = receiver;
    }

    public override async Task<MessageDto?> GetItem(EventPayload eventPayload)
    {
        var client = new MicrosoftOutlookClient(AuthenticationCredentialsProviders);
        var message = await client.Me.Messages[eventPayload.ResourceData.Id].GetAsync();

        if (message == null)

        if (_sender.Email is not null && message?.Sender?.EmailAddress?.Address != _sender.Email)
            return null;

        var receiverEmails = message?.ToRecipients?.Select(r => r.EmailAddress?.Address);
        if (_receiver.Email is not null && receiverEmails is not null && receiverEmails.All(x => x != _receiver.Email))
            return null;
        
        return new MessageDto(message);
    }
}