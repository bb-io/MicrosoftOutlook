using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using Microsoft.Graph.Models;

namespace Apps.MicrosoftOutlook.DataSourceHandlers;

public class MessageDataSourceHandler : BaseInvocable, IAsyncDataSourceHandler
{
    public MessageDataSourceHandler(InvocationContext invocationContext) : base(invocationContext)
    {
    }

    public async Task<Dictionary<string, string>> GetDataAsync(DataSourceContext context,
        CancellationToken cancellationToken)
    {
        IEnumerable<Message> messages;
        if (string.IsNullOrEmpty(context.SearchString))
            messages = await GetRecentMessages(cancellationToken);
        else
            messages = await GetMessages(context.SearchString, cancellationToken);
        
        return messages.ToDictionary(m => m.Id, 
            m => $"{m.Subject} <{m.Sender.EmailAddress.Name} {m.Sender.EmailAddress.Address}>");
    }

    private async Task<IEnumerable<Message>> GetRecentMessages(CancellationToken cancellationToken)
    {
        var client = new MicrosoftOutlookClient(InvocationContext.AuthenticationCredentialsProviders);
        var messages = await client.Me.Messages.GetAsync(requestConfiguration =>
        {
            requestConfiguration.QueryParameters.Top = 20;
            requestConfiguration.QueryParameters.Select = new[] { "id", "subject", "sender" };
        }, cancellationToken);
        return messages.Value;
    }

    private async Task<IEnumerable<Message>> GetMessages(string searchString, CancellationToken cancellationToken)
    {
        var client = new MicrosoftOutlookClient(InvocationContext.AuthenticationCredentialsProviders);
        var messages = await client.Me.Messages.GetAsync(requestConfiguration =>
        {
            requestConfiguration.QueryParameters.Search = searchString;
            requestConfiguration.QueryParameters.Select = new[] { "id", "subject", "sender" };
        }, cancellationToken);
        return messages.Value;
    }
}