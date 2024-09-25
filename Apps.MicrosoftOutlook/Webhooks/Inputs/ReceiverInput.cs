using Blackbird.Applications.Sdk.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.MicrosoftOutlook.Webhooks.Inputs
{
    public class ReceiverInput
    {
        [Display("Receiver email")]
        public string? Email { get; set; }
    }
}
