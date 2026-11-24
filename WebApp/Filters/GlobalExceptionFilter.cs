using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using WebApp.Models;

namespace WebApp.Filters;

public class GlobalExceptionFilter : IExceptionFilter
{
    private readonly ILogger<GlobalExceptionFilter> _log;
    private readonly IHostEnvironment _env;
    private readonly IModelMetadataProvider _meta;

    public GlobalExceptionFilter(
        ILogger<GlobalExceptionFilter> log,
        IHostEnvironment env,
        IModelMetadataProvider meta)
    {
        _log = log;
        _env = env;
        _meta = meta;
    }

    public void OnException(ExceptionContext ctx)
    {
        _log.LogError(ctx.Exception, "Nieobsłużony wyjątek w {Action}", ctx.ActionDescriptor.DisplayName);

        string? detail = null;
        if (_env.IsDevelopment())
        {
            detail = ctx.Exception.Message;
        }

        if (ctx.HttpContext.Request.Path.StartsWithSegments("/api"))
        {
            ProblemDetails problem = new ProblemDetails
            {
                Status = 500,
                Title = "Błąd serwera",
                Detail = detail
            };
            ctx.Result = new ObjectResult(problem)
            {
                StatusCode = 500
            };
        }
        else
        {
            ErrorViewModel vm = new ErrorViewModel
            {
                RequestId = ctx.HttpContext.TraceIdentifier,
                Message = detail,
                Code = 500
            };

            ViewDataDictionary viewData = new ViewDataDictionary(_meta, ctx.ModelState);
            viewData.Model = vm;

            ctx.Result = new ViewResult
            {
                ViewName = "Error",
                StatusCode = 500,
                ViewData = viewData
            };
        }

        ctx.ExceptionHandled = true;
    }
}
