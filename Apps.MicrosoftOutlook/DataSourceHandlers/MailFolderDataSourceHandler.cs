using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.MicrosoftOutlook.DataSourceHandlers;

public class MailFolderDataSourceHandler : BaseInvocable, IAsyncDataSourceItemHandler
{
    public MailFolderDataSourceHandler(InvocationContext invocationContext) : base(invocationContext)
    {
    }

    async Task<IEnumerable<DataSourceItem>> IAsyncDataSourceItemHandler.GetDataAsync(DataSourceContext context, CancellationToken cancellationToken)
    {

        var client = new MicrosoftOutlookClient(InvocationContext.AuthenticationCredentialsProviders);
        var mailFolders = await client.Me.MailFolders.GetAsync(requestConfiguration =>
        {
            requestConfiguration.QueryParameters.Select = new[] { "id", "displayName" };
            requestConfiguration.QueryParameters.Filter = $"contains(displayName, '{context.SearchString ?? ""}')";
        }
            , cancellationToken);

        return mailFolders.Value.Select(f =>new DataSourceItem(f.Id,f.DisplayName));
    }
}