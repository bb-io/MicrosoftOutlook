using Apps.MicrosoftOutlook.Dtos;
using Apps.MicrosoftOutlook.Webhooks.Handlers.Events;
using Apps.MicrosoftOutlook.Webhooks.Lists.ItemGetters;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common.Webhooks;

namespace Apps.MicrosoftOutlook.Webhooks.Lists;

public class EventWebhooks : BaseWebhookList
{
    public EventWebhooks(InvocationContext invocationContext) : base(invocationContext) { }

    [Webhook("On event created", typeof(EventCreatedWebhookHandler), 
        Description = "This webhook is triggered when a new event is created.")]
    public async Task<WebhookResponse<EventDto>> OnMessageCreated(WebhookRequest request)
    {
        return await HandleWebhookRequest(request, new EventGetter(AuthenticationCredentialsProviders));
    }
}