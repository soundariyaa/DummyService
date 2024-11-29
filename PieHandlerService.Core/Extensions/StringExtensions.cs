using System.Text;


namespace PieHandlerService.Core.Extensions;

public static class StringExtensions
{
    public static readonly int DefaultMaskLength = 10;
    public static readonly string MaskedDataAnnotationCharacter = "*";

    public static bool IncludedInIgnoreCase(this string comparer, IEnumerable<string> others) =>
       comparer != null && others.Contains(comparer, StringComparer.OrdinalIgnoreCase);

    public static bool EqualsIgnoreCase(this string comparer, string other) =>
       comparer != null && comparer.Equals(other, StringComparison.OrdinalIgnoreCase);

    public static bool StartsWithIgnoreCase(this string comparer, string other) =>
       comparer != null && comparer.StartsWith(other, StringComparison.OrdinalIgnoreCase);

    public static bool IsAlphaNumericOrWhiteSpace(this string value) =>
        value != null && value.All(c => c.IsAlphaNumeric() || c.IsWhiteSpace());

    public static bool IsHexCharacters(this string value) =>
        value != null && value.All(c => c.IsHexChar());

    public static bool IsAll0(this string value) => value.ToLower().All(x => x.Equals('0'));

    public static bool IsAll0F(this string value) => value.ToLower().All(x => x.Equals('f') || x.Equals('0'));

    /// <summary>
    /// Checks if the string contains only 0-9 characters and does not begin with a zero
    /// </summary>
    public static bool IsNonZeroStartingDigits(this string value) =>
      value.IsDigits() && value.FirstOrDefault() != '0';

    /// <summary>
    /// Checks if string contains only 0-9 characters
    /// </summary>
    public static bool IsDigits(this string value) =>
        value != null && value.All(c => c.IsNumeric());

    public static bool IsValidBase64EncodedString(this string base64EncodedString)
    {
        if (string.IsNullOrWhiteSpace(base64EncodedString))
        {
            return false;
        }
        var buffer = new Span<byte>(new byte[base64EncodedString.Length]);
        return Convert.TryFromBase64String(base64EncodedString, buffer, out _);
    }

    public static string MaskSensitiveData(this string value)
    {
        return value.MaskSensitiveData(DefaultMaskLength);
    }

    public static string MaskSensitiveData(this string value, int maskLength)
    {
        return value.MaskSensitiveData(string.Empty, maskLength, string.Empty);
    }

    public static string MaskSensitiveData(this string value, string prefixWithValue, int maskLength, string postfixWithValue)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return value;
        }

        var stringBuilder = new StringBuilder();
        if (value.Length == 1 || maskLength <= 1)
        {
            stringBuilder.Append(MaskedDataAnnotationCharacter);
            return stringBuilder.ToString();
        }

        var decidedMaskLength = value.Length > maskLength ? maskLength : value.Length;

        if (!string.IsNullOrEmpty(prefixWithValue) && prefixWithValue.Length < decidedMaskLength)
        {
            stringBuilder.Append(prefixWithValue);
            decidedMaskLength -= prefixWithValue.Length;
        }

        if (string.IsNullOrEmpty(postfixWithValue) || postfixWithValue.Length >= decidedMaskLength)
        {
            stringBuilder.Append(new string(MaskedDataAnnotationCharacter[0], decidedMaskLength));
            return stringBuilder.ToString();
        }

        decidedMaskLength -= postfixWithValue.Length;
        stringBuilder.Append(new string(MaskedDataAnnotationCharacter[0], decidedMaskLength));
        stringBuilder.Append(postfixWithValue);

        return stringBuilder.ToString();
    }

    public static string ToBase64(this string input)
    {
        var plainTextBytes = Encoding.UTF8.GetBytes(input);
        return Convert.ToBase64String(plainTextBytes);
    }

    public static bool IsValidHexString(this string data)
    {
        const string hexValueIndication = "0x";
        if (string.IsNullOrWhiteSpace(data))
        {
            return false;
        }
        var value = data;
        if (data.StartsWith(hexValueIndication, StringComparison.OrdinalIgnoreCase))
        {
            value = data.Remove(0, hexValueIndication.Length);
        }

        return value.ToCharArray().All(c => c.IsHexValue());
    }

    public static bool IsNumeric(this string data) =>
        !string.IsNullOrWhiteSpace(data) && data.ToCharArray().All(c => c.IsNumeric());


    public static string HexToAscii(this string hexString)
    {
        var ascii = new StringBuilder();

        for (int i = 0; i < hexString.Length; i += 2)
        {
            var val = hexString.Substring(i, 2);
            ascii.Append(Convert.ToChar(Convert.ToUInt32(val, 16)));
        }

        return ascii.ToString();
    }

    public static string EnsureTrailingSlash(this string input)
    {
        if (string.IsNullOrEmpty(input)) { return input; }
        if (input[^1] == '/') { return input; }
        return input + "/";
    }

    public static string Truncate(this string input, int maxLength, string suffix)
    {
        if (string.IsNullOrEmpty(input)) return input;
        return input.Length <= maxLength ? input : $"{input[..maxLength]}{suffix}";
    }
}
