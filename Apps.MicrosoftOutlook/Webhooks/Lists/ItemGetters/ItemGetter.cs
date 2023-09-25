using Apps.MicrosoftOutlook.Webhooks.Payload;
using Blackbird.Applications.Sdk.Common.Authentication;

namespace Apps.MicrosoftOutlook.Webhooks.Lists.ItemGetters;

public abstract class ItemGetter<T>
{
    protected readonly IEnumerable<AuthenticationCredentialsProvider> AuthenticationCredentialsProviders;
    
    protected ItemGetter(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders)
    {
        AuthenticationCredentialsProviders = authenticationCredentialsProviders;
    }
    
    public abstract Task<T?> GetItem(EventPayload eventPayload);
}