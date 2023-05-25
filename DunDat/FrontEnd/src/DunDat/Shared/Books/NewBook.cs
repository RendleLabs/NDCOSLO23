using System.ComponentModel.DataAnnotations;

namespace DunDat.Shared.Books;

public class NewBook
{
    [Required]
    public string? Title { get; set; }
    [Required]
    public string? Author { get; set; }
}