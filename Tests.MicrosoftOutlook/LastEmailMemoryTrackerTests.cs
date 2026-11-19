using Apps.MicrosoftOutlook.Webhooks.Memory;
using Apps.MicrosoftOutlook.Webhooks.Payload;
using Microsoft.Graph.Models;

namespace Tests.MicrosoftOutlook;

[TestClass]
public class LastEmailMemoryTrackerTests
{
    private static readonly DateTime Now = new(2026, 11, 16, 12, 0, 0, DateTimeKind.Utc);

    [TestMethod]
    public void CreateInitial_StartsFromNow()
    {
        var memory = LastEmailMemoryTracker.CreateInitial(Now);

        Assert.AreEqual(Now, memory.LastEmailDateTime);
        Assert.AreEqual(0, memory.LastMessageIdsAtLastDateTime.Count);
    }

    [TestMethod]
    public void BuildNewEmailsFilter_FiltersByReceivedDateTimeInclusively()
    {
        var filter = LastEmailMemoryTracker.BuildNewEmailsFilter(MemoryAt(Now));

        Assert.AreEqual("receivedDateTime ge 2026-11-16T12:00:00.0000000Z", filter);
        Assert.AreEqual("receivedDateTime asc", LastEmailMemoryTracker.OrderBy);
    }

    [TestMethod]
    public void Advance_EmailWithSentDateInFuture_MovesMemoryToReceivedTimeOnly()
    {
        var memory = MemoryAt(Now.AddMinutes(-10));
        var futureDatedEmail = Email("email-1", sent: Now.AddHours(5), received: Now.AddMinutes(-1));

        var nextMemory = LastEmailMemoryTracker.Advance(memory, [futureDatedEmail]);

        Assert.AreEqual(Now.AddMinutes(-1), nextMemory.LastEmailDateTime);
        CollectionAssert.AreEqual(new[] { "email-1" }, nextMemory.LastMessageIdsAtLastDateTime);
    }

    [TestMethod]
    public void ExcludeAlreadySeen_EmailSentBeforeLastSeenButReceivedAfter_IsKept()
    {
        var memory = MemoryAt(Now.AddMinutes(-5));
        var lateDeliveredEmail = Email("email-1", sent: Now.AddMinutes(-30), received: Now.AddMinutes(-1));

        var newEmails = LastEmailMemoryTracker.ExcludeAlreadySeen([lateDeliveredEmail], memory);

        Assert.AreEqual(1, newEmails.Count);
    }

    [TestMethod]
    public void ExcludeAlreadySeen_EmailsReceivedAtLastSeenTime_SkipsOnlySeenOnes()
    {
        var memory = MemoryAt(Now, "email-1");
        var seenEmail = Email("email-1", sent: Now.AddMinutes(-1), received: Now);
        var unseenEmail = Email("email-2", sent: Now.AddMinutes(-1), received: Now);

        var newEmails = LastEmailMemoryTracker.ExcludeAlreadySeen([seenEmail, unseenEmail], memory);

        CollectionAssert.AreEqual(new[] { "email-2" }, newEmails.Select(email => email.MessageId).ToList());
    }

    [TestMethod]
    public void Normalize_MemoryStuckInFuture_NewEmailIsNotExcluded()
    {
        var memory = LastEmailMemoryTracker.Normalize(MemoryAt(Now.AddDays(2), "email-0"), Now);
        var newEmail = Email("email-1", sent: Now.AddMinutes(1), received: Now.AddMinutes(1));

        var newEmails = LastEmailMemoryTracker.ExcludeAlreadySeen([newEmail], memory);

        Assert.AreEqual(Now, memory.LastEmailDateTime);
        Assert.AreEqual(1, newEmails.Count);
    }

    [TestMethod]
    public void Normalize_MemoryWithoutKind_IsTreatedAsUtc()
    {
        var memory = MemoryAt(DateTime.SpecifyKind(Now.AddMinutes(-5), DateTimeKind.Unspecified), "email-1");

        var normalized = LastEmailMemoryTracker.Normalize(memory, Now);

        Assert.AreEqual(DateTimeKind.Utc, normalized.LastEmailDateTime.Kind);
        Assert.AreEqual(Now.AddMinutes(-5).Ticks, normalized.LastEmailDateTime.Ticks);
        CollectionAssert.AreEqual(new[] { "email-1" }, normalized.LastMessageIdsAtLastDateTime);
    }

    [TestMethod]
    public void Advance_NoNewEmails_KeepsMemory()
    {
        var memory = MemoryAt(Now.AddMinutes(-5), "email-1");

        var nextMemory = LastEmailMemoryTracker.Advance(memory, []);

        Assert.AreSame(memory, nextMemory);
    }

    private static LastEmailMemory MemoryAt(DateTime lastEmailDateTime, params string[] seenIds) => new()
    {
        LastEmailDateTime = lastEmailDateTime,
        LastMessageIdsAtLastDateTime = seenIds.ToList()
    };

    private static ReceivedMessageDto Email(string id, DateTime sent, DateTime received) => new(new Message
    {
        Id = id,
        IsDraft = false,
        SentDateTime = new DateTimeOffset(sent),
        ReceivedDateTime = new DateTimeOffset(received)
    });
}
