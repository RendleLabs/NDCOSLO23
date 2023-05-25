using System.Net.Http.Headers;

namespace DunDat.Clients;

public class AccessTokenProvider
{
    public string? Token { get; set; }

    public void Authenticate(HttpRequestMessage requestMessage)
    {
        requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", Token);
    }
}
