namespace SharedDataverseLogic;

public static class StringExtensions
{
    public static string? Left(this string? str, int maxLength)
    {
        if (str == null)
            return null;

        return str.Length <= maxLength ? str : str.Substring(0, maxLength);
    }
}
