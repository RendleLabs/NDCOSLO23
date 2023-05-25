using System.Runtime.Serialization;
using Azure;
using Azure.Data.Tables;

namespace DunDatApi.Data.Books;

public class BookEntity : ITableEntity
{
    public string PartitionKey { get; set; }
    public string RowKey { get; set; }
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }
    
    public string? AuthorId { get; set; }
    public string Title { get; set; }
    public DateTime FinishedAt { get; set; }
}