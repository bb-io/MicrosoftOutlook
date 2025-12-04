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
        var context = new FolderContentDataSourceContext { FolderId = "AQMkADVjOGQ3MmU0LTY3ZDYtNDE4YS1hZTBhLTA0ZTgwNjdhMTg3ZAAuAAADwZ4c4WrRXEuq8sj71Ok3EAEA51lAbRdl_0WfsI2C11fhLgAAAgEMAAAA" };

        // Act
        var result = await handler.GetFolderContentAsync(context, CancellationToken.None);

        // Assert
        foreach (var item in result)
            Console.WriteLine($"ID: {item.Id}, Name: {item.DisplayName}, Type: {(item.Type is 0 ? "Folder" : "Message")}");
        Assert.IsNotNull(result);
    }
}
