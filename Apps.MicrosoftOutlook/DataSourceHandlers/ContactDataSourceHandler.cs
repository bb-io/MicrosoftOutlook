using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.MicrosoftOutlook.DataSourceHandlers;

public class ContactDataSourceHandler : BaseInvocable, IAsyncDataSourceItemHandler
{
    public ContactDataSourceHandler(InvocationContext invocationContext) : base(invocationContext)
    {
    }

    async Task<IEnumerable<DataSourceItem>> IAsyncDataSourceItemHandler.GetDataAsync(DataSourceContext context, CancellationToken cancellationToken)
    {
        var client = new MicrosoftOutlookClient(InvocationContext.AuthenticationCredentialsProviders);
        var contacts = await client.Me.Contacts.GetAsync(requestConfiguration =>
        {
            requestConfiguration.QueryParameters.Select = new[] { "id", "displayName" };
            requestConfiguration.QueryParameters.Search = context.SearchString ?? " ";
            requestConfiguration.QueryParameters.Top = 20;
        }, cancellationToken);
        return contacts.Value.Select(c => new DataSourceItem(c.Id, c.DisplayName));
    }
}