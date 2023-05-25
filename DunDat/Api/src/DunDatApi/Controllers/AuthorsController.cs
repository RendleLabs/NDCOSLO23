using DunDatApi.Data.Books;
using DunDatApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DunDatApi.Controllers;

[Authorize]
[ApiController]
[Route("read/authors")]
public class AuthorsController : ControllerBase
{
    private readonly AuthorData _authorData;

    public AuthorsController(AuthorData authorData)
    {
        _authorData = authorData;
    }

    [HttpGet(Name = "GetAuthors")]
    [ProducesResponseType(typeof(List<Book>), 200)]
    public async Task<ActionResult<List<Author>>> GetList()
    {
        return await _authorData.GetAsync(User);
    }
}