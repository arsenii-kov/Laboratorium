using Microsoft.AspNetCore.Mvc.Filters;

namespace WebApp.Filters;

public class LogActionFilter : IActionFilter
{
    private readonly ILogger<LogActionFilter> _log;

    public LogActionFilter(ILogger<LogActionFilter> log)
    {
        _log = log;
    }

    public void OnActionExecuting(ActionExecutingContext c)
    {
        List<string> argsList = new List<string>();
        foreach (var item in c.ActionArguments)
        {
            argsList.Add(item.Key + "=" + item.Value);
        }

        string args = string.Join(", ", argsList);
        _log.LogInformation("START {Action}({Args})", c.ActionDescriptor.DisplayName, args);
    }

    public void OnActionExecuted(ActionExecutedContext c)
    {
        _log.LogInformation("KONIEC {Action}", c.ActionDescriptor.DisplayName);
    }
}
