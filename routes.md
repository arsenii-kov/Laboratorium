# Tabela tras aplikacji

| Adres | Kontroler.Akcja | Metoda HTTP |
| --- | --- | --- |
| `/` | Home.Index | ANY |
| `/Home/Index` | Home.Index | ANY |
| `/Home/Privacy` | Home.Privacy | ANY |
| `/docs/{*path}` | Home.Docs | ANY |
| `/Home/Routes` | Home.Routes | ANY |
| `/Home/Error` | Home.Error | ANY |
| `/Book/Hello` | Book.Hello | ANY |
| `/Book/Status` | Book.Status | ANY |
| `/Book/Echo` | Book.Echo | ANY |
| `/Book/Square/{id?}` | Book.Square | ANY |
| `/Book/Import` | Book.Import | POST |
| `/Book/Inspect` | Book.Inspect | ANY |
| `/Book/Sum` | Book.Sum | ANY |
| `/Book/UserAgent` | Book.UserAgent | ANY |
| `/Book/Health` | Book.Health | ANY |
| `/Book/Index` | Book.Index | ANY |
| `/Book/Details/{id:int}` | Book.Details | GET |
| `/Book/Create` | Book.Create | GET |
| `/Book/Create` | Book.Create | POST |
| `/books/year/{year:int:min(1450)}` | Book.ByYear | ANY |
| `/books/isbn/{isbn:regex(^\d{13}$)}` | Book.ByIsbn | ANY |
