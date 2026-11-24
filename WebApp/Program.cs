using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApp.Data;
using WebApp.Filters;
using WebApp.Middleware;
using WebApp.Services;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseStaticWebAssets();

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(delegate (DbContextOptionsBuilder options)
{
    options.UseSqlite(builder.Configuration.GetConnectionString("Default"));
});
builder.Services.AddControllersWithViews(delegate (MvcOptions options)
{
    options.Filters.Add<GlobalExceptionFilter>();
});
builder.Services.AddSingleton<RequestStats>();
builder.Services.AddScoped<LogActionFilter>();
builder.Services.AddScoped<IBookService, BookService>();

var app = builder.Build();

app.UseMiddleware<RequestTimingMiddleware>();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/Home/HttpStatus", "?code={0}");

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "books-by-year",
    pattern: "books/year/{year:int:min(1450)}",
    defaults: new { controller = "Book", action = "ByYear" });

app.MapControllerRoute(
    name: "books-by-isbn",
    pattern: "books/isbn/{isbn:regex(^\\d{{13}}$)}",
    defaults: new { controller = "Book", action = "ByIsbn" });

app.MapControllerRoute(
    name: "docs",
    pattern: "docs/{*path}",
    defaults: new { controller = "Home", action = "Docs" });

app.MapControllerRoute(
    name: "books-archive",
    pattern: "archive/{year:int}/{month:int:range(1,12)}",
    defaults: new { controller = "Book", action = "Archive" });

app.MapControllerRoute(
    name: "admin-area",
    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


app.Run();
