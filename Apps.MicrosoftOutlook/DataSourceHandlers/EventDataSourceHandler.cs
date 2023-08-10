using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using Microsoft.Graph.Models;

namespace Apps.MicrosoftOutlook.DataSourceHandlers;

public class EventDataSourceHandler : BaseInvocable, IAsyncDataSourceHandler
{
    public EventDataSourceHandler(InvocationContext invocationContext) : base(invocationContext)
    {
    }

    public async Task<Dictionary<string, string>> GetDataAsync(DataSourceContext context,
        CancellationToken cancellationToken)
    {
        IEnumerable<Event> events;
        if (string.IsNullOrEmpty(context.SearchString))
            events = await GetEventsFromMainCalendar(cancellationToken);
        else
            events = await GetAllEvents(context.SearchString, cancellationToken);
        
        return events.ToDictionary(e => e.Id, e => e.Subject);
    }

    private async Task<IEnumerable<Event>> GetEventsFromMainCalendar(CancellationToken cancellationToken)
    {
        var client = new MicrosoftOutlookClient(InvocationContext.AuthenticationCredentialsProviders);
        var events = await client.Me.Calendar.Events.GetAsync(requestConfiguration => 
            requestConfiguration.QueryParameters.Select = new[] { "id", "subject" }, cancellationToken);
        return events.Value;
    }
    
    private async Task<IEnumerable<Event>> GetAllEvents(string searchString, CancellationToken cancellationToken)
    {
        var client = new MicrosoftOutlookClient(InvocationContext.AuthenticationCredentialsProviders);
        var calendars = await client.Me.Calendars.GetAsync(requestConfiguration => 
            requestConfiguration.QueryParameters.Select = new[] { "id" }, cancellationToken);
        var events = new List<Event>();

        foreach (var calendar in calendars.Value)
        {
            var calendarEvents = await client.Me.Calendars[calendar.Id].Events.GetAsync(requestConfiguration => 
                requestConfiguration.QueryParameters.Select = new[] { "id", "subject", "body" }, cancellationToken);
            var filteredEvents = calendarEvents.Value
                .Where(e => e.Subject.Contains(searchString, StringComparison.OrdinalIgnoreCase)
                            || e.Body.Content.Contains(searchString, StringComparison.OrdinalIgnoreCase));
            events.AddRange(filteredEvents);
        }

        return events;
    }
}