﻿using Microsoft.Kiota.Abstractions.Authentication;

namespace Apps.MicrosoftOutlook;

public class AccessTokenProvider : IAccessTokenProvider
{
    public string Token { get; set; }

    public AccessTokenProvider(string token)
    {
        Token = token;
    }

    public AllowedHostsValidator AllowedHostsValidator => throw new NotImplementedException();

    public Task<string> GetAuthorizationTokenAsync(Uri uri, Dictionary<string, object>? additionalAuthenticationContext = null, 
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Token);
    }
}