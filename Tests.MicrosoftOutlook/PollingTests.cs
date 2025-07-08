using Apps.MicrosoftOutlook.Webhooks.Lists;
using Apps.MicrosoftOutlook.Webhooks.Memory;
using Apps.MicrosoftOutlook.Webhooks.Payload;
using Blackbird.Applications.Sdk.Common.Polling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tests.MicrosoftOutlook.Base;

namespace Tests.MicrosoftOutlook
{
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
                Memory = null
            };
            var pollinInput = new PollingInput
            {
            };

            var result = await polling.OnEmailsReceived(pollingRequest, pollinInput);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(result, Newtonsoft.Json.Formatting.Indented);
            Console.WriteLine(json);
            Assert.IsNotNull(result);
        }
    }
}
