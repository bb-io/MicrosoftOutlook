using System.Net;
using Apps.MicrosoftOutlook.Dtos;
using Apps.MicrosoftOutlook.Webhooks.Handlers.Mail;
using Apps.MicrosoftOutlook.Webhooks.Payload;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common.Webhooks;
using Newtonsoft.Json;

namespace Apps.MicrosoftOutlook.Webhooks.Lists;

[WebhookList]
public class MailWebhooks : BaseInvocable
{
    public MailWebhooks(InvocationContext invocationContext) : base(invocationContext) { }

    [Webhook("On message created", typeof(MessageCreatedWebhookHandler), 
        Description = "This webhook is triggered when a new message is created.")]
    public async Task<WebhookResponse<MessageDto>> OnMessageCreated(WebhookRequest request)
    {
        return await HandleWebhookRequest(request);
    }
    
    [Webhook("On message updated", typeof(MessageUpdatedWebhookHandler), 
        Description = "This webhook is triggered when a message is updated.")]
    public async Task<WebhookResponse<MessageDto>> OnMessageUpdated(WebhookRequest request)
    {
        return await HandleWebhookRequest(request);
    }

    private async Task<WebhookResponse<MessageDto>> HandleWebhookRequest(WebhookRequest request)
    {
        if (request.QueryParameters.TryGetValue("validationToken", out var validationToken))
        {
            return new WebhookResponse<MessageDto>
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
            return new WebhookResponse<MessageDto>
            {
                HttpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK),
                ReceivedWebhookRequestType = WebhookRequestType.Preflight
            };

        var client = new MicrosoftOutlookClient(InvocationContext.AuthenticationCredentialsProviders);
        var message = await client.Me.Messages[eventPayload.ResourceData.Id].GetAsync();
        
        return new WebhookResponse<MessageDto>
        {
            HttpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK),
            Result = new MessageDto(message)
        };
    }
}