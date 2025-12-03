using Microsoft.Graph.Models;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using Blackbird.Applications.SDK.Extensions.FileManagement.Models.FileDataSourceItems;
using Folder = Blackbird.Applications.SDK.Extensions.FileManagement.Models.FileDataSourceItems.Folder;

namespace Apps.MicrosoftOutlook.DataSourceHandlers;

public class MailFolderDataSourceHandler(InvocationContext invocationContext) 
    : BaseInvocable(invocationContext), IAsyncFileDataSourceItemHandler
{
    public async Task<IEnumerable<FileDataItem>> GetFolderContentAsync(FolderContentDataSourceContext context, CancellationToken ct)
    {
        var client = new MicrosoftOutlookClient(InvocationContext.AuthenticationCredentialsProviders);

        MailFolderCollectionResponse? response;
        if (string.IsNullOrEmpty(context.FolderId))
        {
            response = await client.Me.MailFolders.GetAsync(
                request => request.QueryParameters.Select = ["id", "displayName"], 
                ct
            );
        }
        else
        {
            response = await client.Me.MailFolders[context.FolderId].ChildFolders.GetAsync(
                request => request.QueryParameters.Select = [ "id", "displayName" ], 
                ct
            );
        }

        if (response is null || response.Value is null)
            return [];

        return response.Value.Select(x => new Folder { Id = x.Id!, DisplayName = x.DisplayName!, IsSelectable = true }).ToList();
    }

    public async Task<IEnumerable<FolderPathItem>> GetFolderPathAsync(FolderPathDataSourceContext context, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}