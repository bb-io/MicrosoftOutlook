using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.MicrosoftOutlook.DataSourceHandlers;

public class RecurrencePatternDataSourceHandler : BaseInvocable, IDataSourceItemHandler
{
    public RecurrencePatternDataSourceHandler(InvocationContext invocationContext) : base(invocationContext)
    {
    }

    IEnumerable<DataSourceItem> IDataSourceItemHandler.GetData(DataSourceContext context)
    {
        return new[] { "Daily", "Weekly", "Monthly" }.Select(p => new DataSourceItem(p,p));
    }
}