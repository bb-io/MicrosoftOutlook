using Apps.MicrosoftOutlook.Dtos;
using Apps.MicrosoftOutlook.Webhooks.Inputs;
using Apps.MicrosoftOutlook.Webhooks.Payload;
using Blackbird.Applications.Sdk.Common.Authentication;
using Microsoft.Graph.Models;

namespace Apps.MicrosoftOutlook.Webhooks.Lists.ItemGetters;

public class MessageWithSenderAndAttachmentsGetter: ItemGetter<MessageDto>
{
    private readonly SenderInput _sender;

    public MessageWithSenderAndAttachmentsGetter(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
        SenderInput sender) : base(authenticationCredentialsProviders)
    {
        _sender = sender;
    }

    public override async Task<MessageDto?> GetItem(EventPayload eventPayload)
    {
        var client = new MicrosoftOutlookClient(AuthenticationCredentialsProviders);
        var attachments = await client.Me.Messages[eventPayload.ResourceData.Id].Attachments.GetAsync();
        var fileAttachments = attachments.Value.Where(a => a is FileAttachment);
    
        if (!fileAttachments.Any())
            return null;
        
        var message = await client.Me.Messages[eventPayload.ResourceData.Id].GetAsync();

        if (_sender.Email is not null && message.Sender.EmailAddress.Address != _sender.Email)
            return null;
        
        return new MessageDto(message);
    }
}