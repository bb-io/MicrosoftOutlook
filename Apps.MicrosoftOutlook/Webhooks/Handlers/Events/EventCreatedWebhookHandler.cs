using Apps.MicrosoftOutlook.Webhooks.Inputs;
using Blackbird.Applications.Sdk.Common.Webhooks;

namespace Apps.MicrosoftOutlook.Webhooks.Handlers.Events;

public class EventCreatedWebhookHandler : BaseWebhookHandler
{
    private const string SubscriptionEvent = "created";
    
    public EventCreatedWebhookHandler([WebhookParameter(true)] CalendarInput input) 
        : base(input, SubscriptionEvent) { }
    
    protected override string GetResource()
    {
        var calendarInput = (CalendarInput)WebhookInput;
        var resource = $"/me/calendars/{calendarInput.CalendarId}/events";
        return resource;
    }
}