using DunDatApi.Data;
using DunDatApi.Data.Books;
using DunDatApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DunDatApi.Controllers;

[Authorize]
[ApiController]
[Route("read/books")]
public class BooksController : ControllerBase
{
    private readonly BookData _bookData;

    public BooksController(BookData bookData)
    {
        _bookData = bookData;
    }

    [HttpGet(Name = "GetBooks")]
    [ProducesResponseType(typeof(List<Book>), 200)]
    public async Task<ActionResult<List<Book>>> GetList()
    {
        return await _bookData.GetAsync(User);
    }

    [HttpGet("{id}", Name = "GetBook")]
    [ProducesResponseType(typeof(List<Book>), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    public async Task<ActionResult<Book>> Get(string id)
    {
        try
        {
            return await _bookData.GetAsync(User, id);
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
    }

    /// <summary>
    /// Add a new book that the user has finished.
    /// </summary>
    /// <param name="book">The book details</param>
    /// <returns></returns>
    [HttpPost]
    [ProducesResponseType(typeof(Book), 201)]
    [ProducesResponseType(typeof(ValidationProblemDetails), 400)]
    [ProducesResponseType(typeof(ProblemDetails), 409)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> Add([FromBody] AddBook book)
    {
        var validator = new AddBookValidator();
        var result = validator.Validate(book);
        if (!result.IsValid)
        {
            return BadRequest(result.ToValidationProblemDetails(400, "Book data is not valid"));
        }

        try
        {
            var added = await _bookData.AddAsync(User, book);
            return CreatedAtAction("Get", new { id = added.Id }, added);
        }
        catch (ConflictException)
        {
            return Conflict(new ProblemDetails
            {
                Status = 409,
                Title = "Book already finished",
            });
        }
    }
}

