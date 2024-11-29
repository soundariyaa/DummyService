namespace PieHandlerService.Core.Extensions;

public static class CharExtensions
{
    public static bool IsAlphaNumeric(this char c) => c.IsNumeric() || c.IsAlphabetic();
    public static bool IsAlphabetic(this char c) => c is >= 'A' and <= 'Z' or >= 'a' and <= 'z';
    public static bool IsNumeric(this char c) => c is >= '0' and <= '9';
    public static bool IsHexChar(this char c) => c.IsNumeric() || c is >= 'A' and <= 'F' or >= 'a' and <= 'f';
    public static bool IsWhiteSpace(this char c) => char.IsWhiteSpace(c);
    public static bool IsHexValue(this char c) => IsNumeric(c) || IsHexChar(c);
}
