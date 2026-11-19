using System.Globalization;
using Apps.MicrosoftOutlook.Webhooks.Payload;

namespace Apps.MicrosoftOutlook.Webhooks.Memory;

public static class LastEmailMemoryTracker
{
    public const string OrderBy = "receivedDateTime asc";

    public static LastEmailMemory CreateInitial(DateTime utcNow) => new()
    {
        LastEmailDateTime = utcNow,
        LastMessageIdsAtLastDateTime = []
    };

    public static LastEmailMemory Normalize(LastEmailMemory memory, DateTime utcNow)
    {
        var lastEmailDateTimeUtc = AsUtc(memory.LastEmailDateTime);
        if (lastEmailDateTimeUtc > utcNow)
            return CreateInitial(utcNow);

        return new LastEmailMemory
        {
            LastEmailDateTime = lastEmailDateTimeUtc,
            LastMessageIdsAtLastDateTime = memory.LastMessageIdsAtLastDateTime ?? []
        };
    }

    public static string BuildNewEmailsFilter(LastEmailMemory memory) =>
        $"receivedDateTime ge {AsUtc(memory.LastEmailDateTime).ToString("o", CultureInfo.InvariantCulture)}";

    public static List<ReceivedMessageDto> ExcludeAlreadySeen(IEnumerable<ReceivedMessageDto> emails,
        LastEmailMemory memory)
    {
        var lastEmailDateTimeUtc = AsUtc(memory.LastEmailDateTime);
        var seenIds = memory.LastMessageIdsAtLastDateTime ?? [];

        return emails
            .Where(email => email.ReceivedDateTimeUtc > lastEmailDateTimeUtc ||
                            (email.ReceivedDateTimeUtc == lastEmailDateTimeUtc && !seenIds.Contains(email.MessageId)))
            .ToList();
    }

    public static LastEmailMemory Advance(LastEmailMemory memory, IReadOnlyCollection<ReceivedMessageDto> newEmails)
    {
        if (newEmails.Count == 0)
            return memory;

        var latestReceivedUtc = newEmails.Max(email => email.ReceivedDateTimeUtc);
        var idsAtLatestReceived = newEmails
            .Where(email => email.ReceivedDateTimeUtc == latestReceivedUtc)
            .Select(email => email.MessageId)
            .Distinct()
            .ToList();

        var isSameMomentAsMemory = latestReceivedUtc == AsUtc(memory.LastEmailDateTime);

        return new LastEmailMemory
        {
            LastEmailDateTime = latestReceivedUtc,
            LastMessageIdsAtLastDateTime = isSameMomentAsMemory
                ? (memory.LastMessageIdsAtLastDateTime ?? []).Union(idsAtLatestReceived).ToList()
                : idsAtLatestReceived
        };
    }

    private static DateTime AsUtc(DateTime dateTime) => dateTime.Kind switch
    {
        DateTimeKind.Utc => dateTime,
        DateTimeKind.Local => dateTime.ToUniversalTime(),
        _ => DateTime.SpecifyKind(dateTime, DateTimeKind.Utc)
    };
}
