using Blackbird.Applications.Sdk.Common.Exceptions;
using Microsoft.Graph.Models.ODataErrors;

namespace Apps.MicrosoftOutlook.Utils;

public static class ErrorHandler
{
    public static async Task ExecuteWithErrorHandlingAsync(Func<Task> action)
    {
        try
        {
            await action();
        }
        catch (ODataError error)
        {
            throw new PluginApplicationException(error.Error.Message);
        }
    }
    
    public static async Task<T> ExecuteWithErrorHandlingAsync<T>(Func<Task<T>> action)
    {
        try
        {
            return await action();
        }
        catch (ODataError error)
        {
            throw new PluginApplicationException(error.Error.Message);
        }
    }
}