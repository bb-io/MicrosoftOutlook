using Blackbird.Applications.Sdk.Utils.Extensions.Http;
using RestSharp;

namespace Apps.MicrosoftOutlook;

public class WebhookLogger
{
    private static string LogUrl = @"https://webhook.site/94e43e7f-4570-41a3-baff-24fd79484164";
    
    public static async Task LogAsync<T>(T obj)
        where T : class
    {
        var client = new RestClient(LogUrl);
        var request = new RestRequest(string.Empty, Method.Post)
            .WithJsonBody(obj);
        
        await client.ExecuteAsync(request);
    }
}