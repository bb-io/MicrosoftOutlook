using Apps.MicrosoftOutlook.Webhooks.Inputs;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Webhooks;
using Microsoft.Graph.Models;

namespace Apps.MicrosoftOutlook.Webhooks.Handlers;

public abstract class BaseWebhookHandler : IWebhookEventHandler<IWebhookInput>, IAsyncRenewableWebhookEventHandler
{
    private readonly string _subscriptionEvent; 
    private readonly string _resource;
    protected readonly IWebhookInput webhookInput;

    protected BaseWebhookHandler([WebhookParameter(true)] IWebhookInput input, string subscriptionEvent)
    {
        _subscriptionEvent = subscriptionEvent;
        webhookInput = input;
        _resource = GetResource();
    }

    public async Task SubscribeAsync(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
        Dictionary<string, string> values)
    {
        var client = new MicrosoftOutlookClient(authenticationCredentialsProviders);
        var subscription = new Subscription
        {
            ChangeType = _subscriptionEvent,
            NotificationUrl = values["payloadUrl"],
            Resource = _resource,
            ExpirationDateTime = DateTimeOffset.Now + TimeSpan.FromMinutes(4210),
            ClientState = ApplicationConstants.ClientState
        };
        await client.Subscriptions.PostAsync(subscription);
    }

    public async Task UnsubscribeAsync(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
        Dictionary<string, string> values)
    {
        var client = new MicrosoftOutlookClient(authenticationCredentialsProviders);
        var subscription = (await client.Subscriptions.GetAsync()).Value.First(s => s.NotificationUrl == values["payloadUrl"]);
        await client.Subscriptions[subscription.Id].DeleteAsync();
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