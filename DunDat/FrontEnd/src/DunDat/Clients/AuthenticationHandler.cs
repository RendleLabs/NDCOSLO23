using System.Net.Http.Headers;

namespace DunDat.Clients;

public class AuthenticationHandler : DelegatingHandler
{
    private readonly AccessTokenProvider _accessTokenProvider;

    public AuthenticationHandler(AccessTokenProvider accessTokenProvider)
    {
        _accessTokenProvider = accessTokenProvider;
    }

    public AuthenticationHandler(HttpMessageHandler innerHandler, AccessTokenProvider accessTokenProvider) : base(innerHandler)
    {
        _accessTokenProvider = accessTokenProvider;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (_accessTokenProvider.Token is { Length: > 0 } token)
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
        return base.SendAsync(request, cancellationToken);
    }
}