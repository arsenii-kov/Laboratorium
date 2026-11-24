using Microsoft.EntityFrameworkCore;
using WebApp.Data;
using WebApp.Models;

namespace WebApp.Services;

public class BookService : IBookService
{
    private readonly AppDbContext _db;

    public BookService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<PagedResult<Book>> GetPageAsync(int page, int pageSize, string? search = null, string? sort = null, string? dir = null)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 100);

        IQueryable<Book> q = _db.Books.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
        {
            string term = search.Trim();
            q = q.Where(b => b.Title.Contains(term) || b.Isbn.Contains(term));
        }

        bool desc = string.Equals(dir, "desc", StringComparison.OrdinalIgnoreCase);
        string sortField = "";
        if (sort != null)
        {
            sortField = sort.ToLower();
        }

        if (sortField == "price")
        {
            if (desc)
            {
                q = q.OrderByDescending(b => (double)b.Price);
            }
            else
            {
                q = q.OrderBy(b => (double)b.Price);
            }
        }
        else if (sortField == "date")
        {
            if (desc)
            {
                q = q.OrderByDescending(b => b.PublicationDate);
            }
            else
            {
                q = q.OrderBy(b => b.PublicationDate);
            }
        }
        else
        {
            if (desc)
            {
                q = q.OrderByDescending(b => b.Title);
            }
            else
            {
                q = q.OrderBy(b => b.Title);
            }
        }

        int total = await q.CountAsync();
        List<Book> items = await q.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

        return new PagedResult<Book>(items, page, pageSize, total);
    }

    public async Task<Book?> GetByIdAsync(int id)
    {
        return await _db.Books.AsNoTracking().FirstOrDefaultAsync(b => b.Id == id);
    }

    public async Task<Book?> GetByIsbnAsync(string isbn)
    {
        string normalized = isbn.Replace("-", "").Replace(" ", "");
        List<Book> allBooks = await _db.Books.AsNoTracking().ToListAsync();
        foreach (Book book in allBooks)
        {
            string bookIsbn = book.Isbn.Replace("-", "").Replace(" ", "");
            if (bookIsbn == normalized)
            {
                return book;
            }
        }
        return null;
    }

    public async Task<List<Book>> GetByYearAsync(int year)
    {
        return await _db.Books
            .AsNoTracking()
            .Where(b => b.PublicationDate.HasValue && b.PublicationDate.Value.Year == year)
            .OrderBy(b => b.Title)
            .ToListAsync();
    }

    public async Task<int> GetCountAsync()
    {
        return await _db.Books.CountAsync();
    }

    public async Task<bool> IsbnExistsAsync(string isbn, int? excludeId = null)
    {
        IQueryable<Book> q = _db.Books.AsNoTracking().Where(b => b.Isbn == isbn);
        if (excludeId.HasValue)
        {
            q = q.Where(b => b.Id != excludeId.Value);
        }
        return await q.AnyAsync();
    }

    public async Task<Book> AddAsync(Book book)
    {
        _db.Books.Add(book);
        await _db.SaveChangesAsync();
        return book;
    }

    public async Task<bool> UpdateAsync(Book book)
    {
        Book? existing = await _db.Books.FindAsync(book.Id);
        if (existing == null)
        {
            return false;
        }

        existing.Title = book.Title;
        existing.Isbn = book.Isbn;
        existing.PublicationDate = book.PublicationDate;
        existing.NumPages = book.NumPages;
        existing.Price = book.Price;
        existing.Author = book.Author;

        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        Book? existing = await _db.Books.FindAsync(id);
        if (existing == null)
        {
            return false;
        }

        _db.Books.Remove(existing);
        await _db.SaveChangesAsync();
        return true;
    }
}
