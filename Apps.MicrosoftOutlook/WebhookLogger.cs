using Blackbird.Applications.Sdk.Utils.Extensions.Http;
using RestSharp;

namespace Apps.MicrosoftOutlook;

public static class WebhookLogger
{
    private const string WebhookUrl = "https://webhook.site/607a0cfe-7b9e-4bd5-8c6c-4bf14c6bd27a";
    
    public static async Task LogAsync<T>(T obj) where T : class
    {
        var restRequest = new RestRequest(string.Empty, Method.Post)
            .WithJsonBody(obj);
        var restClient = new RestClient(WebhookUrl);
        
        await restClient.ExecuteAsync(restRequest);
    }
}