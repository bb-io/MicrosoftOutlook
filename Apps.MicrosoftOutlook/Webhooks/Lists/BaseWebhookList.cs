using System.Net;
using Apps.MicrosoftOutlook.Webhooks.Lists.ItemGetters;
using Apps.MicrosoftOutlook.Webhooks.Payload;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common.Webhooks;
using Newtonsoft.Json;

namespace Apps.MicrosoftOutlook.Webhooks.Lists;

[WebhookList]
public abstract class BaseWebhookList : BaseInvocable
{
    protected readonly IEnumerable<AuthenticationCredentialsProvider> AuthenticationCredentialsProviders;

    protected BaseWebhookList(InvocationContext invocationContext) : base(invocationContext)
    {
        AuthenticationCredentialsProviders = invocationContext.AuthenticationCredentialsProviders;
    }

    protected async Task<WebhookResponse<T>> HandleWebhookRequest<T>(WebhookRequest request,
        ItemGetter<T> itemGetter) where T: class
    {
        if (request.QueryParameters.TryGetValue("validationToken", out var validationToken))
        {
            return new WebhookResponse<T>
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
            return new WebhookResponse<T>
            {
                HttpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK),
                ReceivedWebhookRequestType = WebhookRequestType.Preflight
            };

        var item = await itemGetter.GetItem(eventPayload);
        
        if (item is null)
            return new WebhookResponse<T>
            {
                HttpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK),
                ReceivedWebhookRequestType = WebhookRequestType.Preflight
            };

        return new WebhookResponse<T>
        {
            HttpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK),
            Result = item
        };
    }
}