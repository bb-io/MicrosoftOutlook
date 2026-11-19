using Apps.MicrosoftOutlook.Webhooks.Lists;
using Apps.MicrosoftOutlook.Webhooks.Memory;
using Apps.MicrosoftOutlook.Webhooks.Payload;
using Blackbird.Applications.Sdk.Common.Polling;
using Tests.MicrosoftOutlook.Base;

namespace Tests.MicrosoftOutlook;

[TestClass]
public class PollingTests : TestBase
{
    [TestMethod]
    public async Task OnEmailsWithAttachmentsReceived_IsSuccess()
    {
        var polling = new PollingList(InvocationContext);

        var pollingRequest = new PollingEventRequest<LastEmailMemory>
        {
            Memory = null
        };
        var pollinInput = new PollingInput
        {
        };

        var result = await polling.OnEmailsWithAttachmentsReceived(pollingRequest, pollinInput);
         var json = Newtonsoft.Json.JsonConvert.SerializeObject(result, Newtonsoft.Json.Formatting.Indented);
        Console.WriteLine(json);
        Assert.IsNotNull(result);
    }

    [TestMethod]
    public async Task OnEmailsReceived_IsSuccess()
    {
        var polling = new PollingList(InvocationContext);

        var pollingRequest = new PollingEventRequest<LastEmailMemory>
        {
            Memory = new LastEmailMemory { LastEmailDateTime = DateTime.UtcNow.AddMinutes(-6), LastMessageIdsAtLastDateTime = new List<string>()}
        };
        var pollinInput = new PollingInput
        {
        };

        var result = await polling.OnEmailsReceived(pollingRequest, pollinInput);
        var json = Newtonsoft.Json.JsonConvert.SerializeObject(result, Newtonsoft.Json.Formatting.Indented);
        Console.WriteLine(json);
        Assert.IsNotNull(result);
    }

    [TestMethod]
    public async Task OnEmailsReceived_LastDay_FliesWithEmailsReceivedSinceMemoryAndAdvancesMemory()
    {
        var polling = new PollingList(InvocationContext);
        var lastEmailDateTime = DateTime.UtcNow.AddDays(-1);
        var pollingRequest = new PollingEventRequest<LastEmailMemory>
        {
            Memory = new LastEmailMemory { LastEmailDateTime = lastEmailDateTime, LastMessageIdsAtLastDateTime = [] }
        };

        var result = await polling.OnEmailsReceived(pollingRequest, new PollingInput());

        var emails = result.Result?.Emails ?? [];
        Console.WriteLine($"FlyBird: {result.FlyBird}, emails: {emails.Count}, memory: {result.Memory.LastEmailDateTime:o}, ids at memory: {result.Memory.LastMessageIdsAtLastDateTime.Count}");
        Assert.AreEqual(emails.Count > 0, result.FlyBird);
        Assert.IsTrue(emails.All(email => email.ReceivedDateTimeUtc >= lastEmailDateTime));
        Assert.AreEqual(emails.Count, emails.Select(email => email.MessageId).Distinct().Count());
        if (emails.Count > 0)
            Assert.AreEqual(emails.Max(email => email.ReceivedDateTimeUtc), result.Memory.LastEmailDateTime);
    }

    [TestMethod]
    public async Task OnEmailsReceived_SecondPollWithReturnedMemory_DoesNotRepeatEmails()
    {
        var polling = new PollingList(InvocationContext);
        var firstPoll = await polling.OnEmailsReceived(new PollingEventRequest<LastEmailMemory>
        {
            Memory = new LastEmailMemory { LastEmailDateTime = DateTime.UtcNow.AddDays(-1), LastMessageIdsAtLastDateTime = [] }
        }, new PollingInput());

        var secondPoll = await polling.OnEmailsReceived(new PollingEventRequest<LastEmailMemory>
        {
            Memory = firstPoll.Memory
        }, new PollingInput());

        var firstIds = (firstPoll.Result?.Emails ?? []).Select(email => email.MessageId).ToHashSet();
        var repeated = (secondPoll.Result?.Emails ?? []).Count(email => firstIds.Contains(email.MessageId));
        Console.WriteLine($"First poll: {firstIds.Count}, second poll: {secondPoll.Result?.Emails.Count ?? 0}, repeated: {repeated}");
        Assert.AreEqual(0, repeated);
    }

    [TestMethod]
    public async Task OnEmailsWithAttachmentsReceived_LastDay_ReturnsOnlyEmailsReceivedSinceMemory()
    {
        var polling = new PollingList(InvocationContext);
        var lastEmailDateTime = DateTime.UtcNow.AddDays(-1);
        var pollingRequest = new PollingEventRequest<LastEmailMemory>
        {
            Memory = new LastEmailMemory { LastEmailDateTime = lastEmailDateTime, LastMessageIdsAtLastDateTime = [] }
        };

        var result = await polling.OnEmailsWithAttachmentsReceived(pollingRequest, new PollingInput());

        var emails = result.Result?.Emails ?? [];
        Console.WriteLine($"FlyBird: {result.FlyBird}, emails with attachments: {emails.Count}, memory: {result.Memory.LastEmailDateTime:o}");
        Assert.IsTrue(emails.All(email => email.ReceivedDateTimeUtc >= lastEmailDateTime));
        Assert.IsTrue(result.Memory.LastEmailDateTime >= lastEmailDateTime);
    }
}
