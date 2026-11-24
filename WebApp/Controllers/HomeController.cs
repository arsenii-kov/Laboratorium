using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using WebApp.Models;

namespace WebApp.Controllers;

public class HomeController : Controller
{
    private readonly IActionDescriptorCollectionProvider _actionProvider;

    public HomeController(IActionDescriptorCollectionProvider actionProvider)
    {
        _actionProvider = actionProvider;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    public IActionResult Docs(string? path)
    {
        ViewBag.Path = path;
        return View();
    }

    public IActionResult Routes()
    {
        List<RouteInfoViewModel> routes = new List<RouteInfoViewModel>();

        foreach (var item in _actionProvider.ActionDescriptors.Items)
        {
            ControllerActionDescriptor? descriptor = item as ControllerActionDescriptor;
            if (descriptor == null)
            {
                continue;
            }

            string url = "";
            if (descriptor.AttributeRouteInfo != null && !string.IsNullOrEmpty(descriptor.AttributeRouteInfo.Template))
            {
                url = "/" + descriptor.AttributeRouteInfo.Template;
            }
            else
            {
                if (descriptor.RouteValues.ContainsKey("area") && !string.IsNullOrEmpty(descriptor.RouteValues["area"]))
                {
                    url = "/" + descriptor.RouteValues["area"] + "/" + descriptor.ControllerName + "/" + descriptor.ActionName;
                }
                else
                {
                    url = "/" + descriptor.ControllerName + "/" + descriptor.ActionName;
                }
            }

            string methods = "ANY";
            if (descriptor.ActionConstraints != null)
            {
                List<string> methodList = new List<string>();
                foreach (var constraint in descriptor.ActionConstraints)
                {
                    HttpMethodActionConstraint? httpConstraint = constraint as HttpMethodActionConstraint;
                    if (httpConstraint != null)
                    {
                        foreach (string m in httpConstraint.HttpMethods)
                        {
                            methodList.Add(m);
                        }
                    }
                }
                if (methodList.Count > 0)
                {
                    methods = string.Join(", ", methodList);
                }
            }

            RouteInfoViewModel vm = new RouteInfoViewModel
            {
                Url = url,
                Action = descriptor.ControllerName + "." + descriptor.ActionName,
                HttpMethods = methods
            };
            routes.Add(vm);
        }

        return View(routes);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
