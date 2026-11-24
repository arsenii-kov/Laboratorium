namespace WebApp.Models;

public static class AuthorStoreMemory
{
    public static readonly List<Author> Authors = new List<Author>
    {
        new Author { Id = 1, Name = "Stanisław Lem" },
        new Author { Id = 2, Name = "Bolesław Prus" },
        new Author { Id = 3, Name = "Adam Mickiewicz" }
    };
}
