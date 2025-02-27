using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using Microsoft.Graph.Models;

namespace Apps.MicrosoftOutlook.DataSourceHandlers;

public class EventDataSourceHandler : BaseInvocable, IAsyncDataSourceItemHandler
{
    public EventDataSourceHandler(InvocationContext invocationContext) : base(invocationContext)
    {
    }

    private async Task<IEnumerable<Event>> GetEventsFromMainCalendar(CancellationToken cancellationToken)
    {
        var client = new MicrosoftOutlookClient(InvocationContext.AuthenticationCredentialsProviders);
        var events = await client.Me.Calendar.Events.GetAsync(requestConfiguration =>
        {
            requestConfiguration.QueryParameters.Top = 20;
            requestConfiguration.QueryParameters.Select = new[] { "id", "subject" };
        }, cancellationToken);
        return events.Value;
    }
    
    private async Task<IEnumerable<Event>> GetEventsFromAllCalendars(string searchString, CancellationToken cancellationToken)
    {
        var client = new MicrosoftOutlookClient(InvocationContext.AuthenticationCredentialsProviders);
        var calendars = await client.Me.Calendars.GetAsync(requestConfiguration => 
            requestConfiguration.QueryParameters.Select = new[] { "id" }, cancellationToken);
        var events = new List<Event>();

        foreach (var calendar in calendars.Value)
        {
            var calendarEvents = await client.Me.Calendars[calendar.Id].Events.GetAsync(requestConfiguration =>
            {
                requestConfiguration.QueryParameters.Top = 20;
                requestConfiguration.QueryParameters.Filter = $"contains(subject, '{searchString}')";
                requestConfiguration.QueryParameters.Select = new[] { "id", "subject" };
            }, cancellationToken);
            events.AddRange(calendarEvents.Value);
        }

        return events;
    }

    async Task<IEnumerable<DataSourceItem>> IAsyncDataSourceItemHandler.GetDataAsync(DataSourceContext context, CancellationToken cancellationToken)
    {
        IEnumerable<Event> events;
        if (string.IsNullOrEmpty(context.SearchString))
            events = await GetEventsFromMainCalendar(cancellationToken);
        else
            events = await GetEventsFromAllCalendars(context.SearchString, cancellationToken);

        return events.Select(e => new DataSourceItem(e.Id,e.Subject));
    }
}