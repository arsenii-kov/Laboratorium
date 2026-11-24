using Microsoft.AspNetCore.Mvc;
using System.Text;
using WebApp.Models;

namespace WebApp.Controllers;

public class BookController : Controller
{
    // LAB 1

    public IActionResult Hello()
    {
        return Content("Witaj w BookStore!", "text/plain; charset=utf-8");
    }

    public IActionResult Status(int code = 200)
    {
        return StatusCode(code);
    }

    public IActionResult Echo([FromQuery] string name = "nieznajomy", [FromQuery] int count = 1)
    {
        int safeCount = Math.Clamp(count, 1, 20);
        string text = string.Join("\n", Enumerable.Repeat($"Cześć, {name}!", safeCount));
        return Content(text);
    }

    public IActionResult Square(int id)
    {
        return Content($"{id}^2 = {id * id}");
    }

    [HttpPost]
    public IActionResult Import([FromBody] BookDto dto)
    {
        return Json(new { received = dto.Title, dto.Isbn, dto.Price, note = "Tylko demonstracja – nic nie zapisano." });
    }

    public IActionResult Inspect()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine($"Method: {Request.Method}");
        sb.AppendLine($"Path:   {Request.Path}{Request.QueryString}");
        sb.AppendLine($"Scheme: {Request.Scheme}, Host: {Request.Host}");
        sb.AppendLine($"IP:     {HttpContext.Connection.RemoteIpAddress}");
        sb.AppendLine("--- Nagłówki ---");
        foreach (var h in Request.Headers)
        {
            sb.AppendLine($"{h.Key}: {h.Value}");
        }
        return Content(sb.ToString(), "text/plain; charset=utf-8");
    }

    public IActionResult Sum(int a, int b)
    {
        return Content((a + b).ToString());
    }

    public IActionResult UserAgent()
    {
        return Content(Request.Headers.UserAgent.ToString());
    }

    public IActionResult Health()
    {
        return Json(new { status = "ok", time = DateTime.Now });
    }

    // LAB 2

    public IActionResult Index()
    {
        return View(BookStoreMemory.Books);
    }

    public IActionResult Details(int id)
    {
        Book? foundBook = null;
        foreach (Book book in BookStoreMemory.Books)
        {
            if (book.Id == id)
            {
                foundBook = book;
                break;
            }
        }

        if (foundBook == null)
        {
            return NotFound();
        }

        return View(foundBook);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new BookFormViewModel());
    }

    [HttpPost]
    public IActionResult Create(BookFormViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            return View(vm);
        }

        int maxId = 0;
        foreach (Book b in BookStoreMemory.Books)
        {
            if (b.Id > maxId)
            {
                maxId = b.Id;
            }
        }

        Book newBook = new Book
        {
            Id = maxId + 1,
            Title = vm.Title,
            Isbn = vm.Isbn,
            PublicationDate = vm.PublicationDate,
            NumPages = vm.NumPages,
            Price = vm.Price,
            Author = vm.Author
        };
        BookStoreMemory.Books.Add(newBook);

        TempData["Success"] = $"Dodano książkę „{vm.Title}”.";
        return RedirectToAction(nameof(Index));
    }

    // LAB 3 - Część A

    public IActionResult ByYear(int year)
    {
        List<Book> result = new List<Book>();
        foreach (Book book in BookStoreMemory.Books)
        {
            if (book.PublicationDate.HasValue && book.PublicationDate.Value.Year == year)
            {
                result.Add(book);
            }
        }

        ViewData["Title"] = "Książki z roku " + year;
        return View("Index", result);
    }

    public IActionResult ByIsbn(string isbn)
    {
        Book? foundBook = null;
        foreach (Book book in BookStoreMemory.Books)
        {
            string normalized = book.Isbn.Replace("-", "").Replace(" ", "");
            if (normalized == isbn)
            {
                foundBook = book;
                break;
            }
        }

        if (foundBook == null)
        {
            return NotFound();
        }

        return RedirectToAction("Details", new { id = foundBook.Id });
    }
}
