using System.Diagnostics;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WebApp.Filters;

public class MeasureTimeAttribute : ActionFilterAttribute
{
    private const string StopwatchKey = "MeasureTimeStopwatch";

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        Stopwatch sw = Stopwatch.StartNew();
        context.HttpContext.Items[StopwatchKey] = sw;
    }

    public override void OnActionExecuted(ActionExecutedContext context)
    {
        Stopwatch? sw = context.HttpContext.Items[StopwatchKey] as Stopwatch;
        if (sw != null)
        {
            sw.Stop();
            context.HttpContext.Response.Headers["X-Action-Ms"] = sw.ElapsedMilliseconds.ToString();
        }
    }
}
