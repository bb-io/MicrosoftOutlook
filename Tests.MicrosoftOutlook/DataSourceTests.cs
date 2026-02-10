using Tests.MicrosoftOutlook.Base;
using Apps.MicrosoftOutlook.DataSourceHandlers;
using Blackbird.Applications.SDK.Extensions.FileManagement.Models.FileDataSourceItems;
using Apps.MicrosoftOutlook.Actions;
using Apps.MicrosoftOutlook.Models.Mail.Requests;

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

    [TestMethod]
    public async Task GetMessage_ReturnsMailFolders()
    {
        // Arrange
        var handler = new MailActions(InvocationContext, FileManager);
        var context = new GetMessageRequest {MessageId= "AQMkADcxZWFiMWI5LTJkODEtNGRhZC1hM2Q0LTQ1YzA5M2EzYjQ4NgBGAAAD0alzjKXQ4USyBuT9cUNyLAcA2omac5Q2fkOOw3IUCdA63QAAAgEMAAAA2omac5Q2fkOOw3IUCdA63QABK4bFTAAAAA== " };

        // Act
        var result = await handler.GetMessage(InvocationContext.AuthenticationCredentialsProviders, context);

        // Assert
        Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(result));
        Assert.IsNotNull(result);
    }
}
