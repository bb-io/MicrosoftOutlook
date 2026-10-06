using Apps.MicrosoftOutlook.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.SDK.Extensions.FileManagement.Models.FileDataSourceItems;

namespace Apps.MicrosoftOutlook.Models.Mail.Requests;

public class AddMessageCategoryRequest
{
    [Display("Message ID")]
    [FileDataSource(typeof(MessageDataSourceHandler))]
    public string MessageId { get; set; } = string.Empty;

    [DataSource(typeof(CategoryDataSourceHandler))]
    public string Category { get; set; } = string.Empty;
}
