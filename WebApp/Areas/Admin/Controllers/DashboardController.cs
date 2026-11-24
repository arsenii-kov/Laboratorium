using Microsoft.AspNetCore.Mvc;
using WebApp.Middleware;
using WebApp.Models;

namespace WebApp.Areas.Admin.Controllers;

[Area("Admin")]
public class DashboardController : Controller
{
    private readonly RequestStats _stats;

    public DashboardController(RequestStats stats)
    {
        _stats = stats;
    }

    public IActionResult Index()
    {
        ViewBag.BooksCount = BookStoreMemory.Books.Count;
        ViewBag.AuthorsCount = AuthorStoreMemory.Authors.Count;
        ViewBag.TotalRequests = _stats.Total;
        ViewBag.TopPaths = _stats.GetTopPaths(5);
        return View();
    }
}
