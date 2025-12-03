using Microsoft.Graph.Models;
using Apps.MicrosoftOutlook.Utils;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using Blackbird.Applications.SDK.Extensions.FileManagement.Models.FileDataSourceItems;
using Folder = Blackbird.Applications.SDK.Extensions.FileManagement.Models.FileDataSourceItems.Folder;

namespace Apps.MicrosoftOutlook.DataSourceHandlers;

public class MailFolderDataSourceHandler(InvocationContext invocationContext) 
    : BaseInvocable(invocationContext), IAsyncFileDataSourceItemHandler
{
    private const string RootDisplayName = "Mailbox";

    public async Task<IEnumerable<FileDataItem>> GetFolderContentAsync(FolderContentDataSourceContext context, CancellationToken ct)
    {
        var client = new MicrosoftOutlookClient(InvocationContext.AuthenticationCredentialsProviders);

        MailFolderCollectionResponse? response;
        if (string.IsNullOrEmpty(context.FolderId))
        {
            response = await ErrorHandler.ExecuteWithErrorHandlingAsync(async () =>
                await client.Me.MailFolders.GetAsync(
                    request => request.QueryParameters.Select = ["id", "displayName"], 
                    ct
                )
            );
        }
        else
        {
            response = await ErrorHandler.ExecuteWithErrorHandlingAsync(async () =>
                await client.Me.MailFolders[context.FolderId].ChildFolders.GetAsync(
                    request => request.QueryParameters.Select = [ "id", "displayName" ], 
                    ct
                )
            );
        }

        if (response is null || response.Value is null)
            return [];

        List<FileDataItem> folders = [];
        foreach (var folder in response.Value)
            folders.Add(new Folder { Id = folder.Id!, DisplayName = folder.DisplayName!, IsSelectable = true });

        return folders;
    }

    public async Task<IEnumerable<FolderPathItem>> GetFolderPathAsync(FolderPathDataSourceContext context, CancellationToken ct)
    {
        var client = new MicrosoftOutlookClient(InvocationContext.AuthenticationCredentialsProviders);

        if (string.IsNullOrEmpty(context.FileDataItemId))
            return new List<FolderPathItem> { new FolderPathItem { Id = string.Empty, DisplayName = RootDisplayName } };

        var folder = await ErrorHandler.ExecuteWithErrorHandlingAsync(async () => 
            await client.Me.MailFolders[context.FileDataItemId].GetAsync(
                request => request.QueryParameters.Select = ["id", "displayName", "parentFolderId"],
                ct
            )
        );

        var rootFolder = await ErrorHandler.ExecuteWithErrorHandlingAsync(async () =>
            await client.Me.MailFolders["msgfolderroot"].GetAsync(
                request => request.QueryParameters.Select = ["id"],
                ct
            )
        );

        var breadCrumbs = new List<FolderPathItem> { new() { Id = folder!.Id!, DisplayName = folder.DisplayName! } }; 
        var parentFolderId = folder.ParentFolderId;
        while (!string.IsNullOrEmpty(parentFolderId))
        {
            if (parentFolderId == rootFolder!.Id)
                break;

            var parentFolder = await ErrorHandler.ExecuteWithErrorHandlingAsync(async () =>
                await client.Me.MailFolders[parentFolderId].GetAsync(
                    request => request.QueryParameters.Select = ["id", "displayName", "parentFolderId"],
                    ct
                )
            );
            breadCrumbs.Add(new FolderPathItem { Id = parentFolder!.Id!, DisplayName = parentFolder.DisplayName! });
            parentFolderId = parentFolder.ParentFolderId;
        }

        breadCrumbs.Add(new FolderPathItem { Id = string.Empty, DisplayName = RootDisplayName });
        breadCrumbs.Reverse();
        
        return breadCrumbs;
    }
}