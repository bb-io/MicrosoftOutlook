using Apps.MicrosoftOutlook.Constants;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Connections;

namespace Apps.MicrosoftOutlook.Connections;

public class ConnectionDefinition : IConnectionDefinition
{
    public IEnumerable<ConnectionPropertyGroup> ConnectionPropertyGroups =>
    [
        new()
        {
            Name = ConnectionTypes.OAuth,
            AuthenticationType = ConnectionAuthenticationType.OAuth2,
            ConnectionProperties = []
        },
        new()
        {
            Name = ConnectionTypes.OAuthEmailsOnly,
            DisplayName = "OAuth (Send emails only)",
            AuthenticationType = ConnectionAuthenticationType.OAuth2,
            ConnectionProperties = []
        },
    ];

    public IEnumerable<AuthenticationCredentialsProvider> CreateAuthorizationCredentialsProviders(Dictionary<string, string> values)
    {
        string token = values.First(v => v.Key == "access_token").Value;
        var providers = new List<AuthenticationCredentialsProvider> { new("Authorization", token) };
        
        var connectionType = values[nameof(ConnectionPropertyGroup)] switch
        {
            var ct when ConnectionTypes.SupportedConnectionTypes.Contains(ct) => ct,
            _ => throw new Exception($"Unknown connection type: {values[nameof(ConnectionPropertyGroup)]}")
        };

        providers.Add(new AuthenticationCredentialsProvider(CredNames.ConnectionType, connectionType));
        return providers;
    }
}