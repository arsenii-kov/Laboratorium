using Microsoft.AspNetCore.Mvc;
using System.Text;
using WebApp.Models;

namespace WebApp.Controllers;

public class BookController : Controller
{
    // LAB 1

    public IActionResult Hello() => Content("Witaj w BookStore!", "text/plain; charset=utf-8");

    public IActionResult Status(int code = 200) => StatusCode(code);

    public IActionResult Echo([FromQuery] string name = "nieznajomy", [FromQuery] int count = 1) =>
        Content(string.Join("\n", Enumerable.Repeat($"Cześć, {name}!", Math.Clamp(count, 1, 20))));

    public IActionResult Square(int id) => Content($"{id}^2 = {id * id}");

    [HttpPost]
    public IActionResult Import([FromBody] BookDto dto) =>
        Json(new { received = dto.Title, dto.Isbn, dto.Price, note = "Tylko demonstracja – nic nie zapisano." });

    public IActionResult Inspect()
    {
        var sb = new StringBuilder();
        sb.AppendLine($"Method: {Request.Method}");
        sb.AppendLine($"Path:   {Request.Path}{Request.QueryString}");
        sb.AppendLine($"Scheme: {Request.Scheme}, Host: {Request.Host}");
        sb.AppendLine($"IP:     {HttpContext.Connection.RemoteIpAddress}");
        sb.AppendLine("--- Nagłówki ---");
        foreach (var h in Request.Headers) sb.AppendLine($"{h.Key}: {h.Value}");
        return Content(sb.ToString(), "text/plain; charset=utf-8");
    }

    public IActionResult Sum(int a, int b) => Content((a + b).ToString());

    public IActionResult UserAgent() => Content(Request.Headers.UserAgent.ToString());

    public IActionResult Health() => Json(new { status = "ok", time = DateTime.Now });

    // LAB 2

    public IActionResult Index() => View(BookStoreMemory.Books);

    public IActionResult Details(int id)
    {
        var book = BookStoreMemory.Books.FirstOrDefault(b => b.Id == id);
        return book is null ? NotFound() : View(book);
    }

    [HttpGet]
    public IActionResult Create() => View(new BookFormViewModel());

    [HttpPost]
    public IActionResult Create(BookFormViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);

        BookStoreMemory.Books.Add(new Book
        {
            Id = BookStoreMemory.Books.Count > 0 ? BookStoreMemory.Books.Max(b => b.Id) + 1 : 1,
            Title = vm.Title,
            Isbn = vm.Isbn,
            PublicationDate = vm.PublicationDate,
            NumPages = vm.NumPages,
            Price = vm.Price,
            Author = vm.Author
        });

        TempData["Success"] = $"Dodano książkę „{vm.Title}”.";
        return RedirectToAction(nameof(Index));
    }
}
