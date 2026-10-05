using Apps.MicrosoftOutlook.Utils;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.MicrosoftOutlook.DataSourceHandlers;

public class CategoryDataSourceHandler(InvocationContext invocationContext)
    : BaseInvocable(invocationContext), IAsyncDataSourceItemHandler
{
    public async Task<IEnumerable<DataSourceItem>> GetDataAsync(
        DataSourceContext context,
        CancellationToken cancellationToken)
    {
        var client = new MicrosoftOutlookClient(InvocationContext.AuthenticationCredentialsProviders);
        var categories = await ErrorHandler.ExecuteWithErrorHandlingAsync(async () =>
            await client.Me.Outlook.MasterCategories.GetAsync(cancellationToken: cancellationToken));

        return categories?.Value?
                   .Where(category => !string.IsNullOrWhiteSpace(category.DisplayName)
                                      && (string.IsNullOrWhiteSpace(context.SearchString)
                                          || category.DisplayName.Contains(
                                              context.SearchString,
                                              StringComparison.OrdinalIgnoreCase)))
                   .OrderBy(category => category.DisplayName)
                   .Select(category => new DataSourceItem(category.DisplayName!, category.DisplayName!))
               ?? [];
    }
}
