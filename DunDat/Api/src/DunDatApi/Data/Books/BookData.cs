using System.Security.Claims;
using Azure;
using Azure.Data.Tables;
using DunDatApi.Models;

namespace DunDatApi.Data.Books;

public class BookData
{
    private readonly TableServiceClient _tableServiceClient;
    private readonly AuthorData _authorData;
    private readonly ILogger<BookData> _logger;

    public BookData(TableServiceClient tableServiceClient, AuthorData authorData, ILogger<BookData> logger)
    {
        _tableServiceClient = tableServiceClient;
        _authorData = authorData;
        _logger = logger;
    }

    public async Task<List<Book>> GetAsync(ClaimsPrincipal user)
    {
        try
        {
            var (books, authors) = await Tasks.When(GetBooksAsync(user), _authorData.GetAsync(user));
            foreach (var book in books)
            {
                book.Author = authors.FirstOrDefault(a => a.Id! == book.AuthorId);
            }
            return books;
        }
        catch (RequestFailedException ex)
        {
            if (ex.Status == 404) throw new NotFoundException("Book not found", ex);
            throw;
        }

    }

    private async Task<List<Book>> GetBooksAsync(ClaimsPrincipal user)
    {
        var tableClient = GetClient(user);
        var books = new List<Book>();
        var query = tableClient.QueryAsync<BookEntity>(b => b.PartitionKey == "book");
        
        await foreach (var entity in query)
        {
            books.Add(new Book
            {
                Id = entity.RowKey,
                Title = entity.Title,
                FinishedAt = entity.FinishedAt,
                AuthorId = entity.AuthorId,
            });
        }

        return books;
    }

    public async Task<Book> GetAsync(ClaimsPrincipal user, string id)
    {
        var tableClient = GetClient(user);

        try
        {
            var response = await tableClient.GetEntityAsync<BookEntity>("book", id);
            var entity = response.Value;

            var author = await _authorData.GetAsync(user, entity.AuthorId);

            return new Book
            {
                Id = entity.RowKey,
                Title = entity.Title,
                FinishedAt = entity.FinishedAt,
                Author = author,
            };
        }
        catch (RequestFailedException ex)
        {
            if (ex.Status == 404) throw new NotFoundException("Book not found", ex);
            throw;
        }
    }

    public async Task<Book> AddAsync(ClaimsPrincipal user, AddBook book)
    {
        var tableClient = GetClient(user);

        var author = await _authorData.GetOrAddAsync(user, book.Author);

        if (await ExistsAsync(tableClient, book.Title, author))
        {
            throw new ConflictException();
        }

        var bookId = EntityHelpers.GetDescendingRowKey();
        var entity = new BookEntity
        {
            PartitionKey = "book",
            RowKey = bookId,
            Title = book.Title.Trim(),
            AuthorId = author.Id,
            FinishedAt = DateTime.UtcNow,
        };
        await tableClient.AddEntityAsync(entity);
        return new Book
        {
            Id = bookId,
            Author = author,
            Title = entity.Title,
            FinishedAt = entity.FinishedAt
        };
    }

    private async Task<bool> ExistsAsync(TableClient tableClient, string title, Author author)
    {
        var query = tableClient.QueryAsync<BookEntity>(e => e.PartitionKey == "book" && e.AuthorId == author.Id && e.Title == title);

        return await query.GetAsyncEnumerator().MoveNextAsync();
    }

    private TableClient GetClient(ClaimsPrincipal user)
    {
        if (!user.TryGetUserId(out var userId)) throw new InvalidCredentialsException();
        var tableName = TableName.ForUserId(userId);
        return _tableServiceClient.GetTableClient(tableName);
    }
}