using System.Security.Claims;
using Azure;
using Azure.Data.Tables;
using DunDatApi.Models;

namespace DunDatApi.Data.Books;

public class AuthorData
{
    private readonly TableServiceClient _tableServiceClient;
    private readonly ILogger<AuthorData> _logger;

    public AuthorData(TableServiceClient tableServiceClient, ILogger<AuthorData> logger)
    {
        _tableServiceClient = tableServiceClient;
        _logger = logger;
    }

    public async Task<List<Author>> GetAsync(ClaimsPrincipal user)
    {
        var tableClient = GetClient(user);

        try
        {
            var query = tableClient.QueryAsync<AuthorEntity>(e => e.PartitionKey == "author");
            var authors = new List<Author>();

            await foreach (var entity in query)
            {
                authors.Add(new Author
                {
                    Id = entity.RowKey,
                    Name = entity.Name,
                });
            }

            return authors;
        }
        catch (RequestFailedException ex)
        {
            _logger.LogError(ex, "Failed to get Authors for user {Id}", user);
            throw;
        }
    }

    public async Task<Author> GetAsync(ClaimsPrincipal user, string id)
    {
        var tableClient = GetClient(user);

        try
        {
            var response = await tableClient.GetEntityAsync<AuthorEntity>("author", id);
            var entity = response.Value;
            return new Author
            {
                Id = entity.RowKey,
                Name = entity.Name,
            };
        }
        catch (RequestFailedException ex) when (ex.Status == 404)
        {
            _logger.LogError(ex, "Author not found with ID {Id}", id);
            throw new NotFoundException("Author not found", ex);
        }
        catch (RequestFailedException ex)
        {
            _logger.LogError(ex, "Failed to get Author with ID {Id}", id);
            throw;
        }
    }

    public async Task<Author> GetOrAddAsync(ClaimsPrincipal user, string name)
    {
        var tableClient = GetClient(user);

        var query = tableClient.QueryAsync<AuthorEntity>(e => e.PartitionKey == "author" && e.Name == name);

        var entity = await query.FirstOrDefaultAsync();

        if (entity is null)
        {
            var id = Guid.NewGuid().ToString();
            entity = new AuthorEntity
            {
                PartitionKey = "author",
                RowKey = id,
                Name = name,
            };

            await tableClient.AddEntityAsync(entity);
        }

        return new Author
        {
            Id = entity.RowKey,
            Name = entity.Name,
        };
    }

    private TableClient GetClient(ClaimsPrincipal user)
    {
        if (!user.TryGetUserId(out var userId)) throw new InvalidCredentialsException();
        var tableName = TableName.ForUserId(userId);
        return _tableServiceClient.GetTableClient(tableName);
    }
}

public static class AsyncPageableExtensions
{
    public static async Task<T> FirstOrDefaultAsync<T>(this AsyncPageable<T> pageable)
    {
        await using var enumerator = pageable.GetAsyncEnumerator();
        if (!await enumerator.MoveNextAsync()) return default;
        return enumerator.Current;
    }
}