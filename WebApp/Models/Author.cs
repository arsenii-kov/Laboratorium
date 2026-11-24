using System.ComponentModel.DataAnnotations;

namespace WebApp.Models;

public class Author
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Imię i nazwisko jest wymagane")]
    [Display(Name = "Imię i nazwisko")]
    public string Name { get; set; } = "";
}
