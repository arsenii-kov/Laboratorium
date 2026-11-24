using Microsoft.AspNetCore.Mvc;
using System.Text;
using WebApp.Filters;
using WebApp.Models;
using WebApp.Services;

namespace WebApp.Controllers;

[ServiceFilter(typeof(LogActionFilter), Order = 1)]
[MeasureTime(Order = 2)]
[SecondActionFilter(Order = 3)]
public class BookController : Controller
{
    private readonly IBookService _books;

    public BookController(IBookService books)
    {
        _books = books;
    }

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

    // LAB 4 - pełny CRUD + stronicowanie + wyszukiwanie + sortowanie

    public async Task<IActionResult> Index(int page = 1, int pageSize = 5, string? search = null, string? sort = null, string? dir = null)
    {
        PagedResult<Book> result = await _books.GetPageAsync(page, pageSize, search, sort, dir);
        ViewBag.Search = search;
        ViewBag.Sort = sort;
        ViewBag.Dir = dir;
        ViewBag.PageSize = pageSize;
        return View(result);
    }

    [HttpGet("Book/Details/{id:int}")]
    public async Task<IActionResult> Details(int id)
    {
        Book? book = await _books.GetByIdAsync(id);
        if (book == null)
        {
            return NotFound();
        }
        return View(book);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new BookFormViewModel());
    }

    [HttpPost]
    public async Task<IActionResult> Create(BookFormViewModel vm)
    {
        if (await _books.IsbnExistsAsync(vm.Isbn, null))
        {
            ModelState.AddModelError("Isbn", "Książka o podanym numerze ISBN już istnieje.");
        }

        if (!ModelState.IsValid)
        {
            return View(vm);
        }

        Book newBook = new Book
        {
            Title = vm.Title,
            Isbn = vm.Isbn,
            PublicationDate = vm.PublicationDate,
            NumPages = vm.NumPages,
            Price = vm.Price,
            Author = vm.Author
        };

        await _books.AddAsync(newBook);

        TempData["Success"] = $"Dodano książkę „{vm.Title}”.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        Book? book = await _books.GetByIdAsync(id);
        if (book == null)
        {
            return NotFound();
        }

        BookFormViewModel vm = new BookFormViewModel
        {
            Id = book.Id,
            Title = book.Title,
            Isbn = book.Isbn,
            PublicationDate = book.PublicationDate,
            NumPages = book.NumPages,
            Price = book.Price,
            Author = book.Author
        };

        return View(vm);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(int id, BookFormViewModel vm)
    {
        if (id != vm.Id)
        {
            return BadRequest();
        }

        if (await _books.IsbnExistsAsync(vm.Isbn, id))
        {
            ModelState.AddModelError("Isbn", "Inna książka ma już ten numer ISBN.");
        }

        if (!ModelState.IsValid)
        {
            return View(vm);
        }

        Book book = new Book
        {
            Id = vm.Id,
            Title = vm.Title,
            Isbn = vm.Isbn,
            PublicationDate = vm.PublicationDate,
            NumPages = vm.NumPages,
            Price = vm.Price,
            Author = vm.Author
        };

        bool updated = await _books.UpdateAsync(book);
        if (!updated)
        {
            return NotFound();
        }

        TempData["Success"] = $"Zaktualizowano książkę „{vm.Title}”.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        Book? book = await _books.GetByIdAsync(id);
        if (book == null)
        {
            return NotFound();
        }

        return View(book);
    }

    [HttpPost, ActionName("Delete")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        bool deleted = await _books.DeleteAsync(id);
        if (!deleted)
        {
            return NotFound();
        }

        TempData["Success"] = "Usunięto książkę.";
        return RedirectToAction(nameof(Index));
    }

    // LAB 3

    public async Task<IActionResult> ByYear(int year)
    {
        List<Book> books = await _books.GetByYearAsync(year);
        int size = books.Count > 0 ? books.Count : 1;
        PagedResult<Book> paged = new PagedResult<Book>(books, 1, size, books.Count);
        ViewData["Title"] = "Książki z roku " + year;
        return View("Index", paged);
    }

    public async Task<IActionResult> ByIsbn(string isbn)
    {
        Book? foundBook = await _books.GetByIsbnAsync(isbn);
        if (foundBook == null)
        {
            return NotFound();
        }

        return RedirectToAction("Details", new { id = foundBook.Id });
    }

    public async Task<IActionResult> Archive(int year, int month)
    {
        List<Book> books = await _books.GetByYearAsync(year);
        int size = books.Count > 0 ? books.Count : 1;
        PagedResult<Book> paged = new PagedResult<Book>(books, 1, size, books.Count);
        ViewData["Title"] = "Archiwum: " + month + "/" + year;
        return View("Index", paged);
    }

    public IActionResult Boom()
    {
        throw new InvalidOperationException("Celowy wyjątek");
    }
}
