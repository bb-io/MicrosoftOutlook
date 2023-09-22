namespace Apps.MicrosoftOutlook.Webhooks.Payload;

public class EventPayload
{
    public string SubscriptionId { get; set; }
    public string ChangeType { get; set; }
    public string ClientState { get; set; }
    public ResourceData ResourceData { get; set; }
}

public class ResourceData
{
    public string Id { get; set; }
}

public class EventPayloadWrapper
{
    public IEnumerable<EventPayload> Value { get; set; }
}