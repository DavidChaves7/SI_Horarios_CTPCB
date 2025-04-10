
public static class StringExtensions
{
    public static string RemoveSpecialCharacters(this string? value)
    {
        var nValue = value?.Replace("¢", "").Replace("$", "").Replace("%", "").Trim();
        return nValue ?? "";
    }
}
