using Apps.MicrosoftOutlook.Utils;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using Blackbird.Applications.SDK.Extensions.FileManagement.Models.FileDataSourceItems;

namespace Apps.MicrosoftOutlook.DataSourceHandlers;

public class MessageDataSourceHandler(InvocationContext invocationContext) 
    : BaseMailFolderMessagesPicker(invocationContext), IAsyncFileDataSourceItemHandler
{
    public async Task<IEnumerable<FileDataItem>> GetFolderContentAsync(FolderContentDataSourceContext context, CancellationToken ct)
    {
        return await GetFolderContent(context.FolderId, false, true, ct);
    }

    public async Task<IEnumerable<FolderPathItem>> GetFolderPathAsync(FolderPathDataSourceContext context, CancellationToken ct)
    {
        if (string.IsNullOrEmpty(context.FileDataItemId))
            return [new FolderPathItem { Id = string.Empty, DisplayName = RootDisplayName }];

        var client = new MicrosoftOutlookClient(InvocationContext.AuthenticationCredentialsProviders);

        var message = await ErrorHandler.ExecuteWithErrorHandlingAsync(async () =>
            await client.Me.Messages[context.FileDataItemId].GetAsync(
                request => request.QueryParameters.Select = ["parentFolderId"],
                ct
            )
        );
        if (message == null) return [];

        return await BuildParentPathAsync(message.ParentFolderId, ct);
    }
}