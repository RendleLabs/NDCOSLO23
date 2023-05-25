using System.Security.Claims;
using Azure.Data.Tables;
using DunDatApi.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DunDatApi.Controllers;


[Authorize]
[ApiController]
[Route("users")]
public class UserController : ControllerBase
{
    private readonly TableServiceClient _tableServiceClient;

    public UserController(TableServiceClient tableServiceClient)
    {
        _tableServiceClient = tableServiceClient;
    }

    [HttpPut]
    public async Task<IActionResult> Init()
    {
        if (!User.TryGetUserId(out var userId)) return Unauthorized();

        var tableName = TableName.ForUserId(userId);
        var tableClient = _tableServiceClient.GetTableClient(tableName);
        await tableClient.CreateIfNotExistsAsync();
        return Ok();
    }
}