using Apps.MicrosoftOutlook.Webhooks.Inputs;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Webhooks;
using Microsoft.Graph.Models;
using RestSharp;

namespace Apps.MicrosoftOutlook.Webhooks.Handlers;

public abstract class BaseWebhookHandler(string subscriptionEvent)
    : IWebhookEventHandler<IWebhookInput>, IAsyncRenewableWebhookEventHandler
{
    protected readonly IWebhookInput? WebhookInput;

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
            ChangeType = subscriptionEvent,
            NotificationUrl = values["payloadUrl"], //.Replace("https://localhost:44390", "https://fc16-176-36-119-50.ngrok-free.app"),
            Resource = resource,
            ExpirationDateTime = DateTimeOffset.Now + TimeSpan.FromMinutes(4210),
            ClientState = ApplicationConstants.ClientState
        };
        var requestInfo = client.Subscriptions.ToPostRequestInformation(subscription);
        var requestUriAsString = requestInfo.URI.ToString();
        var contentAsString = new StreamReader(requestInfo.Content).ReadToEnd();

        Task.Run(async () =>
        {
            await Task.Delay(1500);

            var client = new RestClient();
            var request = new RestRequest(requestUriAsString, Method.Post);
            request.AddHeader("Authorization", "Bearer " + authenticationCredentialsProviders.First(p => p.KeyName == "Authorization").Value);
            request.AddStringBody(contentAsString, DataFormat.Json);
            await client.ExecuteAsync(request); 
        });

        if (WebhookInput.SharedEmails != null)
        {
            foreach (var sharedContact in WebhookInput.SharedEmails)
            {
                string subscriptionForSharedContact = resource.Replace("/me", $"/users/{sharedContact}");
                var subscriptionShared = new Subscription
                {
                    ChangeType = subscriptionEvent,
                    NotificationUrl = values["payloadUrl"], //.Replace("https://localhost:44390", "https://fc16-176-36-119-50.ngrok-free.app"),
                    Resource = subscriptionForSharedContact,
                    ExpirationDateTime = DateTimeOffset.Now + TimeSpan.FromMinutes(4210),
                    ClientState = ApplicationConstants.ClientState
                };

                var requestSharedInfo = client.Subscriptions.ToPostRequestInformation(subscriptionShared);
                var requestSharedUriAsString = requestInfo.URI.ToString();
                var contentSharedAsString = new StreamReader(requestInfo.Content).ReadToEnd();

                Task.Run(async () =>
                {
                    await Task.Delay(1500);

                    var client = new RestClient();
                    var request = new RestRequest(requestSharedUriAsString, Method.Post);
                    request.AddHeader("Authorization", "Bearer " + authenticationCredentialsProviders.First(p => p.KeyName == "Authorization").Value);
                    request.AddStringBody(contentSharedAsString, DataFormat.Json);
                    await client.ExecuteAsync(request);
                });
                
            }
        }
    }

    public async Task UnsubscribeAsync(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
        Dictionary<string, string> values)
    {
        var client = new MicrosoftOutlookClient(authenticationCredentialsProviders);
        var allSubscriptions = (await client.Subscriptions.GetAsync())!;
        var subscriptions = allSubscriptions.Value!
            .Where(s => s.NotificationUrl == values["payloadUrl"]).ToList(); //.Replace("https://localhost:44390", "https://fc16-176-36-119-50.ngrok-free.app")
        foreach (var subscription in subscriptions)
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