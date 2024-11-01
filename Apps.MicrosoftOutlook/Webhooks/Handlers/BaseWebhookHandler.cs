﻿using Apps.MicrosoftOutlook.Webhooks.Inputs;
using Azure.Core;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Webhooks;
using Microsoft.Graph.Models;
using Newtonsoft.Json;
using RestSharp;
using Tavis.UriTemplates;

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
            NotificationUrl = values["payloadUrl"], //.Replace("https://localhost:44390", "https://cb6f-176-36-119-50.ngrok-free.app"),
            Resource = resource,
            ExpirationDateTime = DateTimeOffset.Now + TimeSpan.FromMinutes(4210),
            ClientState = ApplicationConstants.ClientState
        };
        Task.Run(async () =>
        {
            if (WebhookInput.UrlToSendSubscription != null)
            {
                var clientRest = new RestClient();
                var request = new RestRequest(WebhookInput.UrlToSendSubscription, Method.Post);
                request.AddHeader("Content-Type", "application/json");
                request.AddJsonBody(new
                {
                    beforeDelay = true
                });
                await clientRest.ExecuteAsync(request);
            }

            await Task.Delay(500);

            if (WebhookInput.UrlToSendSubscription != null)
            {
                var clientRest = new RestClient();
                var request = new RestRequest(WebhookInput.UrlToSendSubscription, Method.Post);
                request.AddHeader("Content-Type", "application/json");
                request.AddJsonBody(new
                {
                    beforeExecute = true
                });
                await clientRest.ExecuteAsync(request);
            }

            Subscription result = null;
            try
            {
                result = await client.Subscriptions.PostAsync(subscription);
            }
            catch (Exception ex)
            {
                if (WebhookInput.UrlToSendSubscription != null)
                {
                    var clientRest = new RestClient();
                    var request = new RestRequest(WebhookInput.UrlToSendSubscription, Method.Post);
                    request.AddHeader("Content-Type", "application/json");
                    request.AddJsonBody(new
                    {
                        ex.Message,
                        after = true
                    });
                    await clientRest.ExecuteAsync(request);
                }
            }
            
            if(WebhookInput.UrlToSendSubscription != null)
            {
                var clientRest = new RestClient();
                var request = new RestRequest(WebhookInput.UrlToSendSubscription, Method.Post);
                request.AddHeader("Content-Type", "application/json");
                request.AddJsonBody(new
                {
                    result,
                    after = true
                });
                await clientRest.ExecuteAsync(request);
            }
            
        });

        if (WebhookInput.SharedEmails != null)
        {
            foreach (var sharedContact in WebhookInput.SharedEmails)
            {
                string subscriptionForSharedContact = resource.Replace("/me", $"/users/{sharedContact}");
                var subscriptionShared = new Subscription
                {
                    ChangeType = subscriptionEvent,
                    NotificationUrl = values["payloadUrl"], //.Replace("https://localhost:44390", "https://cb6f-176-36-119-50.ngrok-free.app"),
                    Resource = subscriptionForSharedContact,
                    ExpirationDateTime = DateTimeOffset.Now + TimeSpan.FromMinutes(4210),
                    ClientState = ApplicationConstants.ClientState
                };

                Task.Run(async () =>
                {
                    await Task.Delay(1500);
                    await client.Subscriptions.PostAsync(subscriptionShared);
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
            .Where(s => s.NotificationUrl == values["payloadUrl"]).ToList(); //.Replace("https://localhost:44390", "https://cb6f-176-36-119-50.ngrok-free.app")
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