using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using Microsoft.Graph.Models;

namespace Apps.MicrosoftOutlook.DataSourceHandlers;

public class DraftMessageDataSourceHandler : BaseInvocable, IAsyncDataSourceHandler
{
    public DraftMessageDataSourceHandler(InvocationContext invocationContext) : base(invocationContext)
    {
    }

    public async Task<Dictionary<string, string>> GetDataAsync(DataSourceContext context,
        CancellationToken cancellationToken)
    {
        IEnumerable<Message> messages;
        if (string.IsNullOrEmpty(context.SearchString))
            messages = await GetRecentDraftMessages(cancellationToken);
        else
            messages = await GetDraftMessages(context.SearchString, cancellationToken);
        
        return messages.ToDictionary(m => m.Id, 
            m => $"{m.Subject} <to: {string.Join(", ", m.ToRecipients.Select(r => r.EmailAddress.Address))}>");
    }

    private async Task<IEnumerable<Message>> GetRecentDraftMessages(CancellationToken cancellationToken)
    {
        var client = new MicrosoftOutlookClient(InvocationContext.AuthenticationCredentialsProviders);
        var messages = await client.Me.Messages.GetAsync(requestConfiguration =>
        {
            requestConfiguration.QueryParameters.Top = 20;
            requestConfiguration.QueryParameters.Filter = "isDraft eq true";
            requestConfiguration.QueryParameters.Select = new[] { "id", "subject", "toRecipients" };
        }, cancellationToken);
        return messages.Value;
    }

    private async Task<IEnumerable<Message>> GetDraftMessages(string searchString, CancellationToken cancellationToken)
    {
        var client = new MicrosoftOutlookClient(InvocationContext.AuthenticationCredentialsProviders);
        var messages = await client.Me.Messages.GetAsync(requestConfiguration =>
        {
            requestConfiguration.QueryParameters.Search = searchString;
            requestConfiguration.QueryParameters.Filter = "isDraft eq true";
            requestConfiguration.QueryParameters.Select = new[] { "id", "subject", "toRecipients" };
        }, cancellationToken);
        return messages.Value;
    }
}