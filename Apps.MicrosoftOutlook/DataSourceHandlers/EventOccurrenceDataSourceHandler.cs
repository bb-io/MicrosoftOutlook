using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using Microsoft.Graph.Models;

namespace Apps.MicrosoftOutlook.DataSourceHandlers;

public class EventOccurrenceDataSourceHandler : BaseInvocable, IAsyncDataSourceHandler
{
    public EventOccurrenceDataSourceHandler(InvocationContext invocationContext) : base(invocationContext)
    {
    }

    public async Task<Dictionary<string, string>> GetDataAsync(DataSourceContext context,
        CancellationToken cancellationToken)
    {
        var searchString = context.SearchString;
        var events = await GetUpcomingEventOccurrences(cancellationToken);
        var filteredEvents = events.Where(e => searchString == null 
                                               || e.Subject.Contains(searchString, StringComparison.OrdinalIgnoreCase) 
                                               || e.Body.Content.Contains(searchString, StringComparison.OrdinalIgnoreCase)
                                               || e.Start.ToDateTime().ToLocalTime().ToString("MM/dd/yyyy HH:mm")
                                                   .Contains(searchString, StringComparison.OrdinalIgnoreCase)).Take(20);
        
        return filteredEvents.ToDictionary(e => e.Id, 
            e => $"{e.Start.ToDateTime().ToLocalTime():MM/dd/yyyy HH:mm} {e.Subject}");
    }

    private async Task<IEnumerable<Event>> GetUpcomingEventOccurrences(CancellationToken cancellationToken)
    {
        var client = new MicrosoftOutlookClient(InvocationContext.AuthenticationCredentialsProviders);
        var calendars = await client.Me.Calendars.GetAsync(requestConfiguration => 
            requestConfiguration.QueryParameters.Select = new[] { "id" }, cancellationToken);
        var events = new List<Event>();
        
        foreach (var calendar in calendars.Value)
        {
            EventCollectionResponse? calendarEvents;
            var skipEventsAmount = 0;
            
            do
            {
                calendarEvents = await client.Me.Calendars[calendar.Id].CalendarView.GetAsync(requestConfiguration =>
                {
                    requestConfiguration.QueryParameters.Select = new[] { "id", "subject", "body", "start" };
                    requestConfiguration.QueryParameters.Top = 10;
                    requestConfiguration.QueryParameters.Skip = skipEventsAmount;
                    requestConfiguration.QueryParameters.StartDateTime = DateTime.Now.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss");
                    requestConfiguration.QueryParameters.EndDateTime = (DateTime.Now + TimeSpan.FromDays(30)).ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss");
                }, cancellationToken);
                events.AddRange(calendarEvents.Value);
                skipEventsAmount += 10;
            } while (calendarEvents.OdataNextLink != null);
        }

        var upcomingEvents = events.OrderBy(e => e.Start.ToDateTime());
        return upcomingEvents;
    }
}