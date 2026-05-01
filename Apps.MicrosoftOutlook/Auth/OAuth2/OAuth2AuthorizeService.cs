using Apps.MicrosoftOutlook.Models.Utility;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Authentication.OAuth2;
using Blackbird.Applications.Sdk.Common.Invocation;
using Microsoft.AspNetCore.WebUtilities;

namespace Apps.MicrosoftOutlook.Auth.OAuth2;

public class OAuth2AuthorizeService(InvocationContext invocationContext)
    : BaseInvocable(invocationContext), IOAuth2AuthorizeService
{
    public string GetAuthorizationUrl(Dictionary<string, string> values)
    {
        string bridgeOauthUrl = $"{InvocationContext.UriInfo.BridgeServiceUrl.ToString().TrimEnd('/')}/oauth";
        var oauthCreds = OAuthCredentials.GetOAuthCredentials(values);
        
        var parameters = new Dictionary<string, string?>
        {
            { "client_id", oauthCreds.ClientId },
            { "redirect_uri", $"{InvocationContext.UriInfo.BridgeServiceUrl.ToString().TrimEnd('/')}/AuthorizationCode" },
            { "scope", oauthCreds.Scopes },
            { "state", values["state"] },
            { "response_type", "code" },
            { "authorization_url", oauthCreds.AuthorizeUrl },
            { "actual_redirect_uri", InvocationContext.UriInfo.AuthorizationCodeRedirectUri.ToString() },
        };
        
        return QueryHelpers.AddQueryString(bridgeOauthUrl, parameters);
    }
}