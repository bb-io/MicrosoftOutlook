using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.MicrosoftOutlook.DataSourceHandlers;

public class ContactDataSourceHandler : BaseInvocable, IAsyncDataSourceHandler
{
    public ContactDataSourceHandler(InvocationContext invocationContext) : base(invocationContext)
    {
    }

    public async Task<Dictionary<string, string>> GetDataAsync(DataSourceContext context,
        CancellationToken cancellationToken)
    {
        var client = new MicrosoftOutlookClient(InvocationContext.AuthenticationCredentialsProviders);
        var contacts = await client.Me.Contacts.GetAsync(requestConfiguration => 
            requestConfiguration.QueryParameters.Select = new[] { "id", "displayName" }, cancellationToken);
        return contacts.Value.ToDictionary(c => c.Id, c => c.DisplayName);
    }
}