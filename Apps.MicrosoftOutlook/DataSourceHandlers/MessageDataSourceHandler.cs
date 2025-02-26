using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.MicrosoftOutlook.DataSourceHandlers;

public class MessageDataSourceHandler : BaseInvocable, IAsyncDataSourceItemHandler
{
    public MessageDataSourceHandler(InvocationContext invocationContext) : base(invocationContext)
    {
    }

    async Task<IEnumerable<DataSourceItem>> IAsyncDataSourceItemHandler.GetDataAsync(DataSourceContext context, CancellationToken cancellationToken)
    {
        var client = new MicrosoftOutlookClient(InvocationContext.AuthenticationCredentialsProviders);
        var messages = await client.Me.Messages.GetAsync(requestConfiguration =>
        {
            requestConfiguration.QueryParameters.Top = 20;
            requestConfiguration.QueryParameters.Search = context.SearchString ?? " ";
            requestConfiguration.QueryParameters.Select = new[] { "id", "subject", "sender" };
        }, cancellationToken);

        return messages.Value.Select(m =>new DataSourceItem(m.Id,
            $"{m.Subject} <{m.Sender?.EmailAddress?.Name} {m.Sender?.EmailAddress?.Address}>"));
    }
}