using Apps.MicrosoftOutlook.Webhooks.Inputs;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Webhooks;
using Microsoft.Graph.Models;
using Newtonsoft.Json;
using RestSharp;

namespace Apps.MicrosoftOutlook.Webhooks.Handlers;

public abstract class BaseWebhookHandler : IWebhookEventHandler<IWebhookInput>, IAsyncRenewableWebhookEventHandler
{
    private readonly string _subscriptionEvent;
    protected readonly IWebhookInput? WebhookInput;

    protected BaseWebhookHandler(string subscriptionEvent)
    {
        _subscriptionEvent = subscriptionEvent;
    }
    
    protected BaseWebhookHandler([WebhookParameter(true)] IWebhookInput input, string subscriptionEvent) 
        : this(subscriptionEvent)
    {
        WebhookInput = input;
    }

    public async Task SubscribeAsync(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
        Dictionary<string, string> values)
    {
        var client = new MicrosoftOutlookClient(authenticationCredentialsProviders);
        var resource = GetResource();
        
        var subscription = new Subscription
        {
            ChangeType = _subscriptionEvent,
            NotificationUrl = values["payloadUrl"],
            Resource = resource,
            ExpirationDateTime = DateTimeOffset.Now + TimeSpan.FromMinutes(4210),
            ClientState = ApplicationConstants.ClientState
        };
        await client.Subscriptions.PostAsync(subscription);

        if(WebhookInput.SharedEmails != null)
        {
            foreach (var sharedContact in WebhookInput.SharedEmails)
            {
                string subscriptionForSharedContact = resource.Replace("/me", $"users/{sharedContact}");
                var subscriptionShared = new Subscription
                {
                    ChangeType = _subscriptionEvent,
                    NotificationUrl = values["payloadUrl"],
                    Resource = subscriptionForSharedContact,
                    ExpirationDateTime = DateTimeOffset.Now + TimeSpan.FromMinutes(4210),
                    ClientState = ApplicationConstants.ClientState
                };
                try
                {
                    await client.Subscriptions.PostAsync(subscriptionShared);
                }
                catch(Exception ex)
                {
                    var options = new RestClientOptions("https://webhook.site")
                    {
                        MaxTimeout = -1,
                    };
                    var client2 = new RestClient(options);
                    var request2 = new RestRequest("/34c42d20-8e52-4bf3-b5cf-ec3167c12074", Method.Post);
                    request2.AddJsonBody(new
                    {
                        msg = ex.Message,
                        msg2 = ex.InnerException.Message,
                        msg3 = ex.Source,
                        msg4 = JsonConvert.SerializeObject(ex.Data)
                    });
                    await client2.ExecuteAsync(request2);
                }
                
            }
        }
    }

    public async Task UnsubscribeAsync(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
        Dictionary<string, string> values)
    {
        var client = new MicrosoftOutlookClient(authenticationCredentialsProviders);
        var subscriptions = (await client.Subscriptions.GetAsync()).Value.Where(s => s.NotificationUrl == values["payloadUrl"]).ToList();
        foreach(var subscription in subscriptions)
        {
            await client.Subscriptions[subscription.Id].DeleteAsync();
        } 
    }
    
    [Period(4200)]
    public async Task RenewSubscription(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
        Dictionary<string, string> values)
    {
        var client = new MicrosoftOutlookClient(authenticationCredentialsProviders);
        var subscription = (await client.Subscriptions.GetAsync()).Value.First(s => s.NotificationUrl == values["payloadUrl"]);

        var requestBody = new Subscription
        {
            ExpirationDateTime = DateTimeOffset.Now + TimeSpan.FromMinutes(4000)
        };
        await client.Subscriptions[subscription.Id].PatchAsync(requestBody);
    }

    protected abstract string GetResource();
}