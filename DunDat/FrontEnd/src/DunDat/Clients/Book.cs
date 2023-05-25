namespace DunDat.Clients;

public class Book
{
    public string Id { get; set; }
    public string Title { get; set; }
    public Author Author { get; set; }
    
    public DateTimeOffset FinishedAt { get; set; }
}

public class Author
{
    public string Id { get; set; }
    public string Name { get; set; }
}

public class AddBook
{
    public string Title { get; set; }
    public string Author { get; set; }
}
