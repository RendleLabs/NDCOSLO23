namespace DunDatApi.Data;

public static class EntityHelpers
{
    public static string GetDescendingRowKey() => $"{(long.MaxValue - DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()):0000000000000000000}";
}