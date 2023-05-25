using System.Text.RegularExpressions;

namespace DunDatApi.Data;

public static class TableName
{
    private static readonly Regex NonAlphaNumericRegex = new Regex("[^a-zA-Z0-9]", RegexOptions.Compiled);
    
    public static string ForUserId(string userId) => $"u{StripNonAlphaNumeric(userId)}";

    private static string StripNonAlphaNumeric(string value) => NonAlphaNumericRegex.Replace(value, string.Empty);
}