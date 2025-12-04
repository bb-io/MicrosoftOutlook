using Microsoft.Graph.Models;
using Apps.MicrosoftOutlook.Utils;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Models.FileDataSourceItems;
using File = Blackbird.Applications.SDK.Extensions.FileManagement.Models.FileDataSourceItems.File;
using Folder = Blackbird.Applications.SDK.Extensions.FileManagement.Models.FileDataSourceItems.Folder;

namespace Apps.MicrosoftOutlook.DataSourceHandlers;

public class BaseMailFolderMessagesPicker(InvocationContext invocationContext) : BaseInvocable(invocationContext)
{
    protected const string RootDisplayName = "Mailbox";

    protected async Task<IEnumerable<FileDataItem>> GetFolderContent(
        string? folderId,
        bool foldersAreSelectable,
        bool messagesAreSelectable,
        CancellationToken ct)
    {
        var client = new MicrosoftOutlookClient(InvocationContext.AuthenticationCredentialsProviders);

        MailFolderCollectionResponse? mailFolders;
        MessageCollectionResponse? mailMessages = null;
        if (string.IsNullOrEmpty(folderId))
        {
            mailFolders = await ErrorHandler.ExecuteWithErrorHandlingAsync(async () =>
                await client.Me.MailFolders.GetAsync(
                    request => {
                        request.QueryParameters.Select = ["id", "displayName"];
                        request.QueryParameters.Top = 20;
                    },
                    ct
                )
            );
        }
        else
        {
            mailFolders = await ErrorHandler.ExecuteWithErrorHandlingAsync(async () =>
                await client.Me.MailFolders[folderId].ChildFolders.GetAsync(
                    request => {
                        request.QueryParameters.Select = ["id", "displayName"];
                        request.QueryParameters.Top = 20;
                    },
                    ct
                )
            );
            mailMessages = await ErrorHandler.ExecuteWithErrorHandlingAsync(async () =>
                await client.Me.MailFolders[folderId].Messages.GetAsync(
                    request =>
                    {
                        request.QueryParameters.Select = ["id", "subject", "sender", "receivedDateTime"];
                        request.QueryParameters.Top = 20;
                        request.QueryParameters.Orderby = ["receivedDateTime desc"];
                    },
                    ct
                )
            );
        }

        if (mailFolders is null || mailFolders.Value is null)
            return [];

        List<FileDataItem> result = [];
        foreach (var folder in mailFolders.Value)
        {
            result.Add(
                new Folder 
                { 
                    Id = folder.Id!, 
                    DisplayName = folder.DisplayName!, 
                    IsSelectable = foldersAreSelectable 
                }
            );
        }

        if (mailMessages is not null && mailMessages.Value is not null && mailMessages.Value.Count > 0)
        {
            foreach (var message in mailMessages.Value)
            {
                string senderName = message.Sender?.EmailAddress?.Name ?? "Unknown";
                string senderAddress = message.Sender?.EmailAddress?.Address ?? "";
                string subject = message.Subject ?? "(No Subject)";
                string displayName = $"{subject} <{senderName} {senderAddress}>";

                result.Add(
                    new File
                    {
                        Id = message.Id!,
                        DisplayName = displayName,
                        IsSelectable = messagesAreSelectable,
                        Date = message.ReceivedDateTime?.UtcDateTime,
                    }
                );
            }
        }

        return result;
    }

    protected async Task<List<FolderPathItem>> BuildParentPathAsync(string? startFolderId, CancellationToken ct)
    {
        var client = new MicrosoftOutlookClient(InvocationContext.AuthenticationCredentialsProviders);
        var breadCrumbs = new List<FolderPathItem>();

        var root = await ErrorHandler.ExecuteWithErrorHandlingAsync(async () => 
            await client.Me.MailFolders["msgfolderroot"].GetAsync(
                r => r.QueryParameters.Select = ["id"], 
                ct
            )
        );
        var rootId = root?.Id;

        var currentId = startFolderId;
        while (!string.IsNullOrEmpty(currentId))
        {
            if (currentId == rootId) break;
            if (breadCrumbs.Any(b => b.Id == currentId)) break;

            var folder = await ErrorHandler.ExecuteWithErrorHandlingAsync(async () =>
                await client.Me.MailFolders[currentId].GetAsync(
                    request => request.QueryParameters.Select = ["id", "displayName", "parentFolderId"],
                    ct
                )
            );

            if (folder == null) break;

            breadCrumbs.Add(new FolderPathItem { Id = folder.Id!, DisplayName = folder.DisplayName! });
            currentId = folder.ParentFolderId;
        }

        breadCrumbs.Add(new FolderPathItem { Id = string.Empty, DisplayName = RootDisplayName });
        breadCrumbs.Reverse();

        return breadCrumbs;
    }
}
