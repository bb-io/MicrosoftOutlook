using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.MicrosoftOutlook.DataSourceHandlers;

public class NonDefaultCalendarDataSourceHandler : BaseInvocable, IAsyncDataSourceHandler
{
    public NonDefaultCalendarDataSourceHandler(InvocationContext invocationContext) : base(invocationContext)
    {
    }

    public async Task<Dictionary<string, string>> GetDataAsync(DataSourceContext context,
        CancellationToken cancellationToken)
    {
        var client = new MicrosoftOutlookClient(InvocationContext.AuthenticationCredentialsProviders);
        var calendars = await client.Me.Calendars.GetAsync(requestConfiguration =>
        {
            requestConfiguration.QueryParameters.Select = new[] { "id", "name" };
            requestConfiguration.QueryParameters.Filter = "isDefaultCalendar eq false";
        }, cancellationToken);
        return calendars.Value.ToDictionary(c => c.Id, c => c.Name);
    }
}