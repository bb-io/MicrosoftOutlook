namespace Apps.MicrosoftOutlook.Constants;

public static class ConnectionTypes
{
    public const string OAuth = "OAuth";
    public const string OAuthEmailsOnly = "OAuth (Emails only)";

    public static IEnumerable<string> SupportedConnectionTypes = [OAuth, OAuthEmailsOnly];
}