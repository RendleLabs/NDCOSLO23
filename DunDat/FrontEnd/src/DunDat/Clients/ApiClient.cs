namespace DunDat.Clients;

public class ApiClient
{
    private readonly HttpClient _http;
    private readonly AccessTokenProvider _accessTokenProvider;

    public ApiClient(HttpClient http, AccessTokenProvider accessTokenProvider)
    {
        _http = http;
        _accessTokenProvider = accessTokenProvider;
    }

    public async Task InitUserAsync()
    {
        await PutAsync("/users");
    }

    public async Task<Book[]> GetBooks()
    {
        return await GetAsync<Book[]>("/read/books") ?? Array.Empty<Book>();
    }

    public async Task<Book?> AddBook(string title, string author)
    {
        var book = new AddBook
        {
            Title = title,
            Author = author,
        };

        return await PostAsync<AddBook, Book>("/read/books", book);
    }

    private async Task<T?> GetAsync<T>(string path)
    {
        var request = RequestMessage(HttpMethod.Get, path);
        var response = await _http.SendAsync(request);
        if (!response.IsSuccessStatusCode)
        {
            var responseText = await response.Content.ReadAsStringAsync();
            throw new ApiException(response.StatusCode, responseText);
        }

        return await response.Content.ReadFromJsonAsync<T>();
    }

    private async Task PutAsync(string path)
    {
        var request = RequestMessage(HttpMethod.Put, path);
        var response = await _http.SendAsync(request);
        if (!response.IsSuccessStatusCode)
        {
            var responseText = await response.Content.ReadAsStringAsync();
            throw new ApiException(response.StatusCode, responseText);
        }
    }

    private async Task<TResult?> PostAsync<TContent, TResult>(string path, TContent content)
    {
        var request = RequestMessage(HttpMethod.Post, "/read/books");
        request.Content = JsonContent.Create(content);

        using var response = await _http.SendAsync(request);
        if (response.IsSuccessStatusCode)
        {
            if (response.Headers.Location is { } location)
            {
                return await GetAsync<TResult>(location.ToString());
            }

            return await response.Content.ReadFromJsonAsync<TResult>();
        }

        var responseText = await response.Content.ReadAsStringAsync();
        throw new ApiException(response.StatusCode, responseText);
    }

    private HttpRequestMessage RequestMessage(HttpMethod method, string path)
    {
        var request = new HttpRequestMessage(method, path);
        _accessTokenProvider.Authenticate(request);
        return request;
    }
}