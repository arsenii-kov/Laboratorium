using Microsoft.AspNetCore.Mvc;
using WebApp.Models;

namespace WebApp.Areas.Admin.Controllers;

[Area("Admin")]
public class AuthorsController : Controller
{
    public IActionResult Index()
    {
        return View(AuthorStoreMemory.Authors);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new Author());
    }

    [HttpPost]
    public IActionResult Create(Author author)
    {
        if (!ModelState.IsValid)
        {
            return View(author);
        }

        int maxId = 0;
        foreach (Author a in AuthorStoreMemory.Authors)
        {
            if (a.Id > maxId)
            {
                maxId = a.Id;
            }
        }

        Author newAuthor = new Author
        {
            Id = maxId + 1,
            Name = author.Name
        };
        AuthorStoreMemory.Authors.Add(newAuthor);

        TempData["Success"] = $"Dodano autora „{author.Name}”.";
        return RedirectToAction(nameof(Index));
    }
}
