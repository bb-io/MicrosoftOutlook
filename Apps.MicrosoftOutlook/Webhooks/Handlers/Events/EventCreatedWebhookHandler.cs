using Apps.MicrosoftOutlook.Webhooks.Inputs;
using Blackbird.Applications.Sdk.Common.Webhooks;

namespace Apps.MicrosoftOutlook.Webhooks.Handlers.Events;

public class EventCreatedWebhookHandler : EventBaseWebhookHandler
{
    private const string SubscriptionEvent = "created";
    
    public EventCreatedWebhookHandler([WebhookParameter(true)] CalendarInput input) 
        : base(input, SubscriptionEvent) { }
}