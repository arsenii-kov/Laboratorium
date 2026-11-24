using Microsoft.AspNetCore.Mvc;
using WebApp.Models;

namespace WebApp.Areas.Admin.Controllers;

[Area("Admin")]
public class DashboardController : Controller
{
    public IActionResult Index()
    {
        ViewBag.BooksCount = BookStoreMemory.Books.Count;
        ViewBag.AuthorsCount = AuthorStoreMemory.Authors.Count;
        return View();
    }
}
