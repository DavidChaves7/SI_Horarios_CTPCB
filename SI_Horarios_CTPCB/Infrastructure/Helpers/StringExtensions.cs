
public static class StringExtensions
{
    public static string RemoveSpecialCharacters(this string? value)
    {
        var nValue = value?.Replace("¢", "").Replace("$", "").Replace("%", "").Trim();
        return nValue ?? "";
    }

    public static string OnlyDigits(string? value)
    {
        if (string.IsNullOrEmpty(value)) return string.Empty;
        var sb = new System.Text.StringBuilder();
        foreach (var c in value)
        {
            if (char.IsDigit(c)) sb.Append(c);
        }
        return sb.ToString();
    }
}
