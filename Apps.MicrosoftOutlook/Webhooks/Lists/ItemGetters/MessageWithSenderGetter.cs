using Apps.MicrosoftOutlook.Dtos;
using Apps.MicrosoftOutlook.Webhooks.Inputs;
using Apps.MicrosoftOutlook.Webhooks.Payload;
using Blackbird.Applications.Sdk.Common.Authentication;

namespace Apps.MicrosoftOutlook.Webhooks.Lists.ItemGetters;

public class MessageWithSenderGetter : ItemGetter<MessageDto>
{
    private readonly SenderAndReceiverInput _sender;

    public MessageWithSenderGetter(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
        SenderAndReceiverInput sender) : base(authenticationCredentialsProviders)
    {
        _sender = sender;
    }

    public override async Task<MessageDto?> GetItem(EventPayload eventPayload)
    {
        var client = new MicrosoftOutlookClient(AuthenticationCredentialsProviders);
        var message = await client.Me.Messages[eventPayload.ResourceData.Id].GetAsync();

        if (message == null)

        if (_sender.Email is not null && message?.Sender?.EmailAddress?.Address != _sender.Email)
            return null;

        var receiverEmails = message?.ToRecipients?.Select(r => r.EmailAddress?.Address);
        if (_sender.ReceiverEmail is not null && receiverEmails is not null && receiverEmails.All(x => x != _sender.ReceiverEmail))
            return null;

        return new MessageDto(message);
    }
}