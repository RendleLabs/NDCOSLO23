using System.Text.Json.Serialization;
using FluentValidation;

namespace DunDatApi.Models;

public class Book
{
    public string Id { get; set; }
    public string Title { get; set; }
    public Author? Author { get; set; }
    public DateTimeOffset FinishedAt { get; set; }
    
    [JsonIgnore]
    public string AuthorId { get; set; }
}

public class BookValidator : AbstractValidator<Book>
{
    public BookValidator()
    {
        RuleFor(b => b.Title)
            .NotEmpty();
        RuleFor(b => b.Author)
            .NotNull();
    }
}