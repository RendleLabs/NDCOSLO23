using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;

namespace DunDatApi.Models;

public class Author
{
    public string? Id { get; set; }
    public string Name { get; set; }
}

public class AuthorValidator : AbstractValidator<Author>
{
    public AuthorValidator()
    {
        RuleFor(a => a.Name)
            .NotEmpty();
    }
}

public static class ValidationResultExtensions
{
    public static ValidationProblemDetails ToValidationProblemDetails(this ValidationResult result, int status, string title)
    {
        if (result.IsValid) throw new ArgumentException("Result is valid", nameof(result));

        return new ValidationProblemDetails(result.ToDictionary())
        {
            Status = status,
            Title = title,
        };
    }
}