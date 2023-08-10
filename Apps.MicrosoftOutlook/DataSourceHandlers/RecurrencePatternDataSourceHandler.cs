using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.MicrosoftOutlook.DataSourceHandlers;

public class RecurrencePatternDataSourceHandler : BaseInvocable, IDataSourceHandler
{
    public RecurrencePatternDataSourceHandler(InvocationContext invocationContext) : base(invocationContext)
    {
    }

    public Dictionary<string, string> GetData(DataSourceContext context)
    {
        return new[] { "Daily", "Weekly", "Monthly" }.ToDictionary(p => p, p => p);
    }
}