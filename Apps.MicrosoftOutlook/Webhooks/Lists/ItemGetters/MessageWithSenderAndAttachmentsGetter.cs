using Apps.MicrosoftOutlook.Dtos;
using Apps.MicrosoftOutlook.Webhooks.Inputs;
using Apps.MicrosoftOutlook.Webhooks.Payload;
using Blackbird.Applications.Sdk.Common.Authentication;
using Microsoft.Graph.Models;
using System.Reflection;

namespace Apps.MicrosoftOutlook.Webhooks.Lists.ItemGetters;

public class MessageWithSenderAndAttachmentsGetter: ItemGetter<MessageDto>
{
    private readonly SenderAndReceiverInput _sender;

    public MessageWithSenderAndAttachmentsGetter(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
        SenderAndReceiverInput sender) : base(authenticationCredentialsProviders)
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

        var receiverEmails = message?.ToRecipients?.Select(r => r.EmailAddress?.Address);
        if (_sender.ReceiverEmail is not null && receiverEmails is not null && receiverEmails.All(x => x != _sender.ReceiverEmail))
            return null;

        return new MessageDto(message);
    }
}