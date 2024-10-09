using Apps.MicrosoftOutlook.Webhooks.Inputs;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Webhooks;
using Microsoft.Graph.Models;
using Newtonsoft.Json;
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
        try
        {
            await WebhookLogger.LogAsync(new { status = "subscribing", values });

            var client = new MicrosoftOutlookClient(authenticationCredentialsProviders);
            var resource = GetResource();

            var subscription = new Subscription
            {
                ChangeType = subscriptionEvent,
                NotificationUrl = values["payloadUrl"],
                Resource = resource,
                ExpirationDateTime = DateTimeOffset.Now + TimeSpan.FromMinutes(4210),
                ClientState = ApplicationConstants.ClientState
            };
            await client.Subscriptions.PostAsync(subscription);

            if (WebhookInput.SharedEmails != null)
            {
                foreach (var sharedContact in WebhookInput.SharedEmails)
                {
                    string subscriptionForSharedContact = resource.Replace("/me", $"/users/{sharedContact}");
                    var subscriptionShared = new Subscription
                    {
                        ChangeType = subscriptionEvent,
                        NotificationUrl = values["payloadUrl"],
                        Resource = subscriptionForSharedContact,
                        ExpirationDateTime = DateTimeOffset.Now + TimeSpan.FromMinutes(4210),
                        ClientState = ApplicationConstants.ClientState
                    };

                    await Task.Delay(4000);
                    
                    await client.Subscriptions.PostAsync(subscriptionShared);
                }
            }

            await WebhookLogger.LogAsync(new { status = "subscribed", values });
        }
        catch (Exception e)
        {
            await WebhookLogger.LogAsync(new { status = "error", values, error = e.Message, error_type = e.GetType().ToString(), stack_trace = e.StackTrace });
            throw;
        }
    }

    public async Task UnsubscribeAsync(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
        Dictionary<string, string> values)
    {
        try
        {
            await WebhookLogger.LogAsync(new { status = "unsubscribing", values });
            
            var client = new MicrosoftOutlookClient(authenticationCredentialsProviders);
            var allSubscriptions = (await client.Subscriptions.GetAsync())!;
            var subscriptions = allSubscriptions.Value!
                .Where(s => s.NotificationUrl == values["payloadUrl"]).ToList();
            foreach (var subscription in subscriptions)
            {
                await client.Subscriptions[subscription.Id].DeleteAsync();
            }
            
            await WebhookLogger.LogAsync(new { status = "unsubscribed", values });
        }
        catch (Exception e)
        {            
            await WebhookLogger.LogAsync(new { status = "error", values, error = e.Message, error_type = e.GetType().ToString(), stack_trace = e.StackTrace });
            throw;
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