using Microsoft.AspNetCore.Mvc.Filters;

namespace WebApp.Filters;

public class SecondActionFilterAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        ILogger<SecondActionFilterAttribute>? logger =
            context.HttpContext.RequestServices.GetService(typeof(ILogger<SecondActionFilterAttribute>)) as ILogger<SecondActionFilterAttribute>;

        if (logger != null)
        {
            logger.LogInformation("SECOND FILTER START (Order={Order}) {Action}", Order, context.ActionDescriptor.DisplayName);
        }
    }

    public override void OnActionExecuted(ActionExecutedContext context)
    {
        ILogger<SecondActionFilterAttribute>? logger =
            context.HttpContext.RequestServices.GetService(typeof(ILogger<SecondActionFilterAttribute>)) as ILogger<SecondActionFilterAttribute>;

        if (logger != null)
        {
            logger.LogInformation("SECOND FILTER KONIEC (Order={Order}) {Action}", Order, context.ActionDescriptor.DisplayName);
        }
    }
}
