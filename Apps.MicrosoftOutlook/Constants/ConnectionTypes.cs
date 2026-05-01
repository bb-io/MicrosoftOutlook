namespace Apps.MicrosoftOutlook.Constants;

public static class ConnectionTypes
{
    public const string OAuth = "OAuth";
    public const string OAuthAzure = "OAuth (Azure app)";

    public static IEnumerable<string> SupportedConnectionTypes = [OAuth, OAuthAzure];
}