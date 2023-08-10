using Apps.MicrosoftOutlook.DataSourceHandlers;
using Apps.MicrosoftOutlook.Dtos;
using Apps.MicrosoftOutlook.Models.Event.Requests;
using Apps.MicrosoftOutlook.Models.Event.Responses;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Dynamic;
using HtmlAgilityPack;
using Microsoft.Graph.Models;
using Microsoft.Graph.Models.ODataErrors;
using Microsoft.Kiota.Abstractions;

namespace Apps.MicrosoftOutlook.Actions;

[ActionList]
public class EventActions
{
    private const string EventBodyContentId = "EventBodyContentId";
    
    #region GET

    [Action("Calendar: list events", Description = "Retrieve a list of events in a calendar. If calendar is not " +
                                                   "specified, default calendar's events are listed.")]
    public async Task<ListEventsResponse> ListEvents(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
        [ActionParameter] ListEventsRequest request) 
    {
        var client = new MicrosoftOutlookClient(authenticationCredentialsProviders);
        EventCollectionResponse? events;
        try
        {
            if (request == null || request.CalendarId == null)
                events = await client.Me.Calendar.Events.GetAsync();
            else 
                events = await client.Me.Calendars[request.CalendarId].Events.GetAsync();
        }
        catch (ODataError error)
        {
            throw new ArgumentException(error.Error.Message);
        }
        return new ListEventsResponse
        {
            Events = events.Value.Select(e => new EventDto(e))
        };
    }
    
    [Action("Calendar: get event", Description = "Get information about an event.")]
    public async Task<EventDto> GetEvent(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
        [ActionParameter] GetEventRequest request) 
    {
        var client = new MicrosoftOutlookClient(authenticationCredentialsProviders);
        try
        {
            var eventData = await client.Me.Events[request.EventId].GetAsync();
            var eventDto = new EventDto(eventData);
            return eventDto;
        }
        catch (ODataError error)
        {
            throw new ArgumentException(error.Error.Message);
        }
    }
    
    [Action("Calendar: list occurrences of event", Description = "Get the occurrences of an event for a specified time range.")]
    public async Task<ListEventsResponse> ListEventOccurrences(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
        [ActionParameter] ListEventOccurrencesRequest request) 
    {
        var client = new MicrosoftOutlookClient(authenticationCredentialsProviders);
        try
        {
            var events = await client.Me.Events[request.EventId].Instances.GetAsync(requestConfiguration =>
                {
                    requestConfiguration.QueryParameters.StartDateTime = request.StartDate.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss");
                    requestConfiguration.QueryParameters.EndDateTime = request.EndDate.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss");
                });
            return new ListEventsResponse
            {
                Events = events.Value.Select(e => new EventDto(e))
            };
        }
        catch (ODataError error)
        {
            throw new ArgumentException(error.Error.Message);
        }
    }
    
    [Action("Calendar: list recently created events", Description = "Retrieve a list of events created during past hours. " +
                                                                    "If number of hours is not specified, events created " +
                                                                    "during past 24 hours are listed. If calendar is not " +
                                                                    "specified, default calendar's events are listed.")]
    public async Task<ListEventsResponse> ListRecentlyCreatedEvents(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
        [ActionParameter] ListRecentlyCreatedEventsRequest request) 
    {
        var client = new MicrosoftOutlookClient(authenticationCredentialsProviders);
        EventCollectionResponse? events;
        var startDateTime = (DateTime.Now - TimeSpan.FromHours(request.Hours ?? 24)).ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");
        var requestFilter = $"createdDateTime ge {startDateTime}";
        try
        {
            if (request == null || request.CalendarId == null)
                events = await client.Me.Calendar.Events.GetAsync(requestConfiguration => 
                    requestConfiguration.QueryParameters.Filter = requestFilter);
            else
                events = await client.Me.Calendars[request.CalendarId].Events.GetAsync(requestConfiguration =>
                    requestConfiguration.QueryParameters.Filter = requestFilter);
        }
        catch (ODataError error)
        {
            throw new ArgumentException(error.Error.Message);
        }
        return new ListEventsResponse
        {
            Events = events.Value.Select(e => new EventDto(e))
        };
    }

    [Action("Calendar: list recently updated events", Description = "Retrieve a list of events updated during past hours. " +
                                                                    "If number of hours is not specified, events updated " +
                                                                    "during past 24 hours are listed. If calendar is not " +
                                                                    "specified, default calendar's events are listed.")]
    public async Task<ListEventsResponse> ListRecentlyUpdatedEvents(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
        [ActionParameter] ListRecentlyUpdatedEventsRequest request) 
    {
        var client = new MicrosoftOutlookClient(authenticationCredentialsProviders);
        EventCollectionResponse? events;
        var startDateTime = (DateTime.Now - TimeSpan.FromHours(request.Hours ?? 24)).ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");
        var requestFilter = $"lastModifiedDateTime ge {startDateTime}";
        try
        {
            if (request == null || request.CalendarId == null)
                events = await client.Me.Calendar.Events.GetAsync(requestConfiguration => 
                    requestConfiguration.QueryParameters.Filter = requestFilter);
            else 
                events = await client.Me.Calendars[request.CalendarId].Events.GetAsync(requestConfiguration => 
                    requestConfiguration.QueryParameters.Filter = requestFilter);
        }
        catch (ODataError error)
        {
            throw new ArgumentException(error.Error.Message);
        }
        return new ListEventsResponse
        {
            Events = events.Value.Select(e => new EventDto(e))
        };
    }
    
    #endregion
    
    #region POST
    
    [Action("Calendar: create event in a calendar", Description = "Create a new event in a calendar. If calendar is not " +
                                                                  "specified, the event is created in the default calendar. " +
                                                                  "If the event is an online meeting, a Microsoft Teams " +
                                                                  "meeting is automatically created. To create a recurring " +
                                                                  "event specify recurrence pattern and interval which " +
                                                                  "can be in days, weeks or months, depending on recurrence " +
                                                                  "pattern type. If interval is not specified, it is " +
                                                                  "set to 1. For weekly or monthly patterns provide days " +
                                                                  "of week on which the event occurs.")]
    public async Task<EventDto> CreateEvent(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
        [ActionParameter] CreateEventRequest request)
    {
        var daysOfWeek = new Dictionary<string, DayOfWeekObject>(StringComparer.OrdinalIgnoreCase)
        {
            { "sunday", DayOfWeekObject.Sunday },
            { "monday", DayOfWeekObject.Monday },
            { "tuesday", DayOfWeekObject.Tuesday },
            { "wednesday", DayOfWeekObject.Wednesday },
            { "thursday", DayOfWeekObject.Thursday },
            { "friday", DayOfWeekObject.Friday },
            { "saturday", DayOfWeekObject.Saturday }
        };

        if (!IsValidTimeFormat(request.StartTime, out TimeSpan startTime) 
            || !IsValidTimeFormat(request.EndTime, out TimeSpan endTime)) 
            throw new ArgumentException("Time format is not valid.");

        if (request.RecurrencePattern != null)
        {
            if (request.Interval < 1) 
                throw new ArgumentException("Recurrence interval must be greater than zero.");
            
            if (request.RecurrencePattern != "Daily" && (request.DaysOfWeek == null || !request.DaysOfWeek.Any()))
                throw new ArgumentException("For weekly and monthly recurrence patterns days of week should be specified.");

            if (request.RecurrencePattern != "Daily")
            {
                foreach (var day in request.DaysOfWeek)
                {
                    var isValidDayOfWeek = daysOfWeek.Keys.Any(d => d == day.ToLower());
                    if (!isValidDayOfWeek)
                        throw new ArgumentException($"Day of week '{day}' is not valid.");
                }
            }
            else
                request.DaysOfWeek = Array.Empty<string>();
        }

        var client = new MicrosoftOutlookClient(authenticationCredentialsProviders);
        var requestBody = new Event
        {
            Subject = request.Subject,
            Body = new ItemBody
            {
                ContentType = BodyType.Html,
                Content = WrapEventBodyContent(request.BodyContent)
            },
            Start = new DateTimeTimeZone
            {
                DateTime = request.EventDate.ToString("yyyy-MM-dd") + $"T{startTime}",
                TimeZone = TimeZoneInfo.Local.Id
            },
            End = new DateTimeTimeZone
            {
                DateTime = request.EventDate.ToString("yyyy-MM-dd") + $"T{endTime}",
                TimeZone = TimeZoneInfo.Local.Id
            },
            Location = new Location
            {
                DisplayName = request.Location ?? (request.IsOnlineMeeting ? "Microsoft Teams Meeting" : "No location specified")
            },
            IsOnlineMeeting = request.IsOnlineMeeting,
            IsReminderOn = request.IsReminderOn,
            ReminderMinutesBeforeStart = request.ReminderMinutesBeforeStart ?? (request.IsReminderOn ? 15 : -1),
            Attendees = new List<Attendee>(request.AttendeeEmails.Select(email => new Attendee
            {
                EmailAddress = new EmailAddress { Address = email },
                Type = AttendeeType.Optional
            })),
            Recurrence = request.RecurrencePattern == null ? null : new PatternedRecurrence
            {
                Pattern = new RecurrencePattern
                {
                    Type = (RecurrencePatternType)Enum.Parse(typeof(RecurrencePatternType), request.RecurrencePattern),
                    DaysOfWeek = new List<DayOfWeekObject?>(request.DaysOfWeek.Select(d => daysOfWeek[d] as DayOfWeekObject?)),
                    Interval = request.Interval ?? 1
                },
                Range = new RecurrenceRange
                {
                    Type = request.RecurrenceEndDate == null ? RecurrenceRangeType.NoEnd : RecurrenceRangeType.EndDate,
                    StartDate = new Date(request.EventDate.Year, request.EventDate.Month, request.EventDate.Day),
                    EndDate = request.RecurrenceEndDate == null ? 
                        new Date(DateTime.MinValue.Year, DateTime.MinValue.Month, DateTime.MinValue.Day) 
                        : new Date(request.RecurrenceEndDate.Value.Year, request.RecurrenceEndDate.Value.Month, request.RecurrenceEndDate.Value.Day)
                    
                }
            }
        };

        Event? createdEvent;
        try
        {
            if (request.CalendarId == null)
                createdEvent = await client.Me.Calendar.Events.PostAsync(requestBody);
            else 
                createdEvent = await client.Me.Calendars[request.CalendarId].Events.PostAsync(requestBody);
        }
        catch (ODataError error)
        {
            throw new ArgumentException(error.Error.Message);
        }
        
        var createdEventDto = new EventDto(createdEvent);
        return createdEventDto;
    }

    [Action("Calendar: cancel event", Description = "This action allows the organizer of a meeting to send a cancellation " +
                                                    "message and cancel the event.")]
    public async Task CancelEvent(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
        [ActionParameter] [Display("Event")] [DataSource(typeof(EventDataSourceHandler))] string eventId,
        [ActionParameter] CancelEventRequest request)
    {
        await CancelEventOrEventOccurrence(authenticationCredentialsProviders, eventId, request);
    }
    
    [Action("Calendar: cancel event occurrence", Description = "This action allows the organizer of a meeting to send " +
                                                               "a cancellation message and cancel an occurrence of a " +
                                                               "recurring meeting.")]
    public async Task CancelEventOccurrence(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
        [ActionParameter] [Display("Event occurrence")] [DataSource(typeof(EventOccurrenceDataSourceHandler))] string eventOccurrenceId,
        [ActionParameter] CancelEventRequest request)
    {
        await CancelEventOrEventOccurrence(authenticationCredentialsProviders, eventOccurrenceId, request);
    }

    [Action("Calendar: forward event", Description = "This action allows the organizer or attendee of a meeting event " +
                                                     "to forward the meeting request to a new recipient.")]
    public async Task ForwardEvent(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
        [ActionParameter] ForwardEventRequest request)
    {
        var client = new MicrosoftOutlookClient(authenticationCredentialsProviders);
        var requestBody = new Microsoft.Graph.Me.Events.Item.Forward.ForwardPostRequestBody
        {
            ToRecipients = new List<Recipient>
            {
                new Recipient
                {
                    EmailAddress = new EmailAddress
                    {
                        Address = request.RecipientEmail,
                        Name = request.RecipientName ?? ""
                    }
                }
            },
            Comment = request.Comment ?? ""
        };
        try
        {
            await client.Me.Events[request.EventId].Forward.PostAsync(requestBody);
        }
        catch (ODataError error)
        {
            throw new ArgumentException(error.Error.Message);
        }
    }

    #endregion
    
    #region PATCH

    [Action("Calendar: update event", Description = "Update an existing event. Specify fields that need to be updated.")]
    public async Task<EventDto> UpdateEvent(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
        [ActionParameter] [Display("Event")] [DataSource(typeof(EventDataSourceHandler))] string eventId,
        [ActionParameter] UpdateEventRequest request)
    {
        return await UpdateEventOrEventOccurrence(authenticationCredentialsProviders, eventId, request);
    }
    
    [Action("Calendar: update event occurrence", Description = "Update an existing occurrence of a recurring event. " +
                                                               "Specify fields that need to be updated.")]
    public async Task<EventDto> UpdateEventOccurrence(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
        [ActionParameter] [Display("Event occurrence")] [DataSource(typeof(EventOccurrenceDataSourceHandler))] string eventOccurrenceId,
        [ActionParameter] UpdateEventRequest request)
    {
        return await UpdateEventOrEventOccurrence(authenticationCredentialsProviders, eventOccurrenceId, request);
    }

    #endregion

    #region DELETE

    [Action("Calendar: delete event", Description = "Delete an event.")]
    public async Task DeleteEvent(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
        [ActionParameter] DeleteEventRequest request) 
    {
        var client = new MicrosoftOutlookClient(authenticationCredentialsProviders);
        try
        {
            await client.Me.Events[request.EventId].DeleteAsync();
        }
        catch (ODataError error)
        {
            throw new ArgumentException(error.Error.Message);
        }
    }

    #endregion

    private async Task CancelEventOrEventOccurrence(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
        string eventOrEventOccurrenceId, CancelEventRequest request)
    {
        var client = new MicrosoftOutlookClient(authenticationCredentialsProviders);
        var requestBody = new Microsoft.Graph.Me.Events.Item.Cancel.CancelPostRequestBody
        {
            Comment = request.Comment ?? ""
        };
        try
        {
            await client.Me.Events[eventOrEventOccurrenceId].Cancel.PostAsync(requestBody);
        }
        catch (ODataError error)
        {
            throw new ArgumentException(error.Error.Message);
        }
    }
    
    private async Task<EventDto> UpdateEventOrEventOccurrence(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
        string eventOrEventOccurrenceId, UpdateEventRequest request)
    {
        string UpdateBodyContentWithOnlineMeetingInformation(string html, string newContent)
        {
            var document = new HtmlDocument();
            document.LoadHtml(html);
            var bodyContent = document.GetElementbyId(EventBodyContentId);
            bodyContent.InnerHtml = newContent;
            return document.DocumentNode.InnerHtml;
        }

        string RecalculateContent(Event existingEvent)
        {
            string content;
            if (existingEvent.IsOnlineMeeting.Value && 
                (request.IsOnlineMeeting == null || request.IsOnlineMeeting.Value)) 
                content = UpdateBodyContentWithOnlineMeetingInformation(existingEvent.Body.Content, request.BodyContent);
            else 
                content = WrapEventBodyContent(request.BodyContent);
            return content;
        }

        string UpdateDate(string originalDateString, DateTime? newDate, string? newTime)
        {
            var isValidTimeFormat = !IsValidTimeFormat(newTime, out var parsedTime);
            if (newTime != null && isValidTimeFormat)
                throw new ArgumentException("Time format is not valid.");

            var originalDateTime = DateTime.Parse(originalDateString).ToLocalTime();
            var updatedDate = (newDate?.ToString("yyyy-MM-dd") ?? originalDateTime.ToString("yyyy-MM-dd")) + "T" 
                + (newTime != null ? parsedTime : originalDateTime.TimeOfDay);
            return updatedDate;
        }

        var client = new MicrosoftOutlookClient(authenticationCredentialsProviders);
        try
        {
            var eventData = await client.Me.Events[eventOrEventOccurrenceId].GetAsync();
            eventData.Subject = request.Subject ?? eventData.Subject;
            eventData.Body = request.BodyContent != null
                ? new ItemBody { ContentType = BodyType.Html, Content = RecalculateContent(eventData) }
                : eventData.Body;
            eventData.Start.DateTime = UpdateDate(eventData.Start.DateTime, request.EventDate, request.StartTime);
            eventData.Start.TimeZone = TimeZoneInfo.Local.Id;
            eventData.End.DateTime = UpdateDate(eventData.End.DateTime, request.EventDate, request.EndTime);
            eventData.End.TimeZone = TimeZoneInfo.Local.Id;
            eventData.Location = request.Location != null
                ? new Location { DisplayName = request.Location }
                : eventData.Location;
            eventData.IsOnlineMeeting = request.IsOnlineMeeting ?? eventData.IsOnlineMeeting;
            eventData.IsReminderOn = request.IsReminderOn ?? eventData.IsReminderOn;
            eventData.ReminderMinutesBeforeStart = request.ReminderMinutesBeforeStart ?? eventData.ReminderMinutesBeforeStart;
            eventData.Attendees = request.AttendeeEmails != null
                ? new List<Attendee>(request.AttendeeEmails.Select(email => new Attendee
                {
                    EmailAddress = new EmailAddress { Address = email },
                    Type = AttendeeType.Optional
                }))
                : eventData.Attendees;
            
            var updatedEvent = await client.Me.Events[eventOrEventOccurrenceId].PatchAsync(eventData);
            var updatedEventDto = new EventDto(updatedEvent);
            return updatedEventDto;
        }
        catch (ODataError error)
        {
            throw new ArgumentException(error.Error.Message);
        }
    }
    
    private string WrapEventBodyContent(string? content)
    {
        return $"<div id='{EventBodyContentId}'>{content ?? ""}</div>";
    }

    private bool IsValidTimeFormat(string time, out TimeSpan parsedTime)
    {;
        return TimeSpan.TryParse(time, out parsedTime);
    }
}