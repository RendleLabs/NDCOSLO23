// ReSharper disable once CheckNamespace

using System.Diagnostics.CodeAnalysis;

namespace System.Security.Claims;

public static class ClaimsPrincipalExtensions
{
    public static bool TryGetUserId(this ClaimsPrincipal principal, [NotNullWhen(true)] out string? userId)
    {
        var claim = principal.FindFirst(ClaimTypes.NameIdentifier);
        userId = claim?.Value;
        return userId is not null;
    }
}