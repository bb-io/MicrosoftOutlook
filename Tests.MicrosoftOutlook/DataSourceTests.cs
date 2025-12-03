using Tests.MicrosoftOutlook.Base;
using Apps.MicrosoftOutlook.DataSourceHandlers;
using Blackbird.Applications.SDK.Extensions.FileManagement.Models.FileDataSourceItems;

namespace Tests.MicrosoftOutlook;

[TestClass]
public class DataSourceTests : TestBase
{
    [TestMethod]
    public async Task MailFolderDataSourceHandler_ReturnsMailFolders()
    {
        // Arrange
        var handler = new MailFolderDataSourceHandler(InvocationContext);
        var context = new FolderContentDataSourceContext { FolderId = "" };

        // Act
        var result = await handler.GetFolderContentAsync(context, CancellationToken.None);

        // Assert
        foreach (var item in result)
            Console.WriteLine($"ID: {item.Id}, Name: {item.DisplayName}");
        Assert.IsNotNull(result);
    }
}
