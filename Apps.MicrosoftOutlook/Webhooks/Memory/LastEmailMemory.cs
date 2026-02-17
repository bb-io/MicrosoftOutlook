namespace Apps.MicrosoftOutlook.Webhooks.Memory;
public class LastEmailMemory
{
    public DateTime LastEmailDateTime { get; set; }
    public List<string> LastMessageIdsAtLastDateTime { get; set; } = new();
}

