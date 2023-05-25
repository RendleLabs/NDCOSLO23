using FluentValidation;

namespace DunDatApi.Models;

public class AddBook
{
    public string Title { get; set; }
    public string Author { get; set; }
}

public class AddBookValidator : AbstractValidator<AddBook>
{
    public AddBookValidator()
    {
        RuleFor(b => b.Title)
            .NotEmpty();
        RuleFor(b => b.Author)
            .NotNull();
    }
}
