
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using YamlDotNet.Core.Tokens;

public static class StringExtensions
{

    const int keySize = 64;
    const int iterations = 320000;
    static HashAlgorithmName hashAlgorithm = HashAlgorithmName.SHA512;

    public static string HashPasword(this string password, out byte[] salt)
    {
        salt = RandomNumberGenerator.GetBytes(keySize);
        var hash = Rfc2898DeriveBytes.Pbkdf2(Encoding.UTF8.GetBytes(password),
        salt,
        iterations,
            hashAlgorithm,
            keySize);
        return Convert.ToHexString(hash);
    }

    public static bool VerifyPassword(this string password, string hash, byte[] salt)
    {
        var hashToCompare = Rfc2898DeriveBytes.Pbkdf2(password, salt, iterations, hashAlgorithm, keySize);
        return CryptographicOperations.FixedTimeEquals(hashToCompare, Convert.FromHexString(hash));
    }

    public static string ReduceStringDecimals(this string value, int decimalsNumbers = 2)
    {
        return value.Substring(0, value.IndexOf('.') + decimalsNumbers + 1);
    }

    public static string? RemoveSpecialCharacters(this string? value, bool normalizeDecimals = false)
    {
        var nValue = value?.Replace("¢", "").Replace("$", "").Replace("%", "").Trim();
        if (!string.IsNullOrEmpty(nValue))
        {
            if (normalizeDecimals)
            {
                var temp = double.Parse(nValue, new NumberFormatInfo() { NumberDecimalSeparator = ",", NumberGroupSeparator = "." });
                var normalValue = Convert.ToDouble(temp, CultureInfo.GetCultureInfo("en-US"));
                return normalValue.ToString();
            }
        }
        return null;
    }

}
