﻿using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.MicrosoftOutlook.DataSourceHandlers;

public class NonDefaultCalendarDataSourceHandler : BaseInvocable, IAsyncDataSourceItemHandler
{
    public NonDefaultCalendarDataSourceHandler(InvocationContext invocationContext) : base(invocationContext)
    {
    }

    async Task<IEnumerable<DataSourceItem>> IAsyncDataSourceItemHandler.GetDataAsync(DataSourceContext context, CancellationToken cancellationToken)
    {
        var client = new MicrosoftOutlookClient(InvocationContext.AuthenticationCredentialsProviders);
        var calendars = await client.Me.Calendars.GetAsync(requestConfiguration =>
        {
            requestConfiguration.QueryParameters.Select = new[] { "id", "name" };
            requestConfiguration.QueryParameters.Filter = "isDefaultCalendar eq false";
        }, cancellationToken);

        return calendars.Value
            .Where(c => context.SearchString == null
                        || c.Name.Contains(context.SearchString, StringComparison.OrdinalIgnoreCase))
            .Select(c =>new DataSourceItem(c.Id,c.Name));
    }
}