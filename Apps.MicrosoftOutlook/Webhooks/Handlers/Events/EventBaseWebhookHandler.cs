using Apps.MicrosoftOutlook.Webhooks.Inputs;
using Blackbird.Applications.Sdk.Common.Webhooks;

namespace Apps.MicrosoftOutlook.Webhooks.Handlers.Events;

public class EventBaseWebhookHandler : BaseWebhookHandler
{
    protected EventBaseWebhookHandler([WebhookParameter(true)] CalendarInput input, string subscriptionEvent)
        : base(input, subscriptionEvent) { }
    
    protected override string GetResource()
    {
        var calendarInput = (CalendarInput)webhookInput;
        var resource = $"/me/calendars/{calendarInput.CalendarId}/events";
        return resource;
    }
}