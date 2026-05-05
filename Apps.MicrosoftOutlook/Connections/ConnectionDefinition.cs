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
            Name = ConnectionTypes.OAuthAzure,
            DisplayName = "OAuth (Azure app)",
            AuthenticationType = ConnectionAuthenticationType.OAuth2,
            ConnectionProperties = [
                new(CredNames.AzureClientId) { DisplayName = "Application (client) ID" },
                new(CredNames.AzureTenantId) { DisplayName = "Directory (tenant) ID" },
                new(CredNames.AzureClientSecret) { DisplayName = "Client secret", Sensitive = true },
                new(CredNames.EmailsOnly) 
                { 
                    DisplayName = "Only allow email sending scopes",
                    DataItems = 
                    [
                        new("yes", "Yes"),
                        new("no", "No")
                    ]
                }
            ]
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