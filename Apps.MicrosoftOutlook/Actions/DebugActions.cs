using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.MicrosoftOutlook.Actions;

[ActionList("Debug")]
public class DebugActions(InvocationContext invocationContext) : BaseInvocable(invocationContext)
{
    [Action("Debug", Description = "Debug")]
    public List<AuthenticationCredentialsProvider> Debug()
    {
        return InvocationContext.AuthenticationCredentialsProviders.ToList();
    }
}
