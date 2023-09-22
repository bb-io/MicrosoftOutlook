using System.Net;
using Apps.MicrosoftOutlook.Dtos;
using Apps.MicrosoftOutlook.Webhooks.Handlers.Events;
using Apps.MicrosoftOutlook.Webhooks.Payload;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common.Webhooks;
using Newtonsoft.Json;

namespace Apps.MicrosoftOutlook.Webhooks.Lists;

[WebhookList]
public class EventWebhooks : BaseInvocable
{
    public EventWebhooks(InvocationContext invocationContext) : base(invocationContext) { }

    [Webhook("On event created", typeof(EventCreatedWebhookHandler), 
        Description = "This webhook is triggered when a new event is created.")]
    public async Task<WebhookResponse<EventDto>> OnMessageCreated(WebhookRequest request)
    {
        return await HandleWebhookRequest(request);
    }
    
    private async Task<WebhookResponse<EventDto>> HandleWebhookRequest(WebhookRequest request)
    {
        if (request.QueryParameters.TryGetValue("validationToken", out var validationToken))
        {
            return new WebhookResponse<EventDto>
            {
                HttpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(validationToken)
                },
                ReceivedWebhookRequestType = WebhookRequestType.Preflight
            };
        }
        
        var eventPayload = JsonConvert.DeserializeObject<EventPayloadWrapper>(request.Body.ToString(), 
            new JsonSerializerSettings { MissingMemberHandling = MissingMemberHandling.Ignore }).Value.First();

        if (eventPayload.ClientState != ApplicationConstants.ClientState)
            return new WebhookResponse<EventDto>
            {
                HttpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK),
                ReceivedWebhookRequestType = WebhookRequestType.Preflight
            };

        var client = new MicrosoftOutlookClient(InvocationContext.AuthenticationCredentialsProviders);
        var calendarEvent = await client.Me.Events[eventPayload.ResourceData.Id].GetAsync();
        
        return new WebhookResponse<EventDto>
        {
            HttpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK),
            Result = new EventDto(calendarEvent)
        };
    }
}