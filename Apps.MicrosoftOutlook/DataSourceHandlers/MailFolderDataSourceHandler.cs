using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.MicrosoftOutlook.DataSourceHandlers;

public class MailFolderDataSourceHandler : BaseInvocable, IAsyncDataSourceHandler
{
    public MailFolderDataSourceHandler(InvocationContext invocationContext) : base(invocationContext)
    {
    }

    public async Task<Dictionary<string, string>> GetDataAsync(DataSourceContext context,
        CancellationToken cancellationToken)
    {
        var client = new MicrosoftOutlookClient(InvocationContext.AuthenticationCredentialsProviders);
        var mailFolders = await client.Me.MailFolders.GetAsync(requestConfiguration => 
            requestConfiguration.QueryParameters.Select = new[] { "id", "displayName" }, cancellationToken);
        
        return mailFolders.Value
            .Where(f => context.SearchString == null 
                             || f.DisplayName.Contains(context.SearchString, StringComparison.OrdinalIgnoreCase))
            .ToDictionary(f => f.Id, f => f.DisplayName);
    }
}