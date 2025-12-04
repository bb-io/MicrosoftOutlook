using Apps.MicrosoftOutlook.Utils;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using Blackbird.Applications.SDK.Extensions.FileManagement.Models.FileDataSourceItems;

namespace Apps.MicrosoftOutlook.DataSourceHandlers;

public class MailFolderDataSourceHandler(InvocationContext invocationContext) 
    : BaseMailFolderMessagesPicker(invocationContext), IAsyncFileDataSourceItemHandler
{
    public async Task<IEnumerable<FileDataItem>> GetFolderContentAsync(FolderContentDataSourceContext context, CancellationToken ct)
    {
        return await GetFolderContent(context.FolderId, true, false, ct);
    }

    public async Task<IEnumerable<FolderPathItem>> GetFolderPathAsync(FolderPathDataSourceContext context, CancellationToken ct)
    {
        if (string.IsNullOrEmpty(context.FileDataItemId))
            return [new FolderPathItem { Id = string.Empty, DisplayName = RootDisplayName }];

        var client = new MicrosoftOutlookClient(InvocationContext.AuthenticationCredentialsProviders);

        var currentFolder = await ErrorHandler.ExecuteWithErrorHandlingAsync(async () =>
            await client.Me.MailFolders[context.FileDataItemId].GetAsync(
                request => request.QueryParameters.Select = ["id", "displayName", "parentFolderId"],
                ct
            )
        );
        if (currentFolder == null) return [];

        return await BuildParentPathAsync(currentFolder.ParentFolderId, ct);
    }
}