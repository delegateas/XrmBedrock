using System.Globalization;

namespace SharedDomain;

public static class CultureInfoHelperExtensions
{
    private static readonly CultureInfo SolutionDefaultCulture = new CultureInfo("da-DK");

    /// <summary>
    ///  Returns string value of the intput using Solution/Customer default Culture Info.
    /// </summary>
    /// <param name="value">Input for conversion.</param>
    /// <returns>String value of input.</returns>
    public static string ToStringSolutionDefault(this DateTime value)
    {
        return value.ToString(SolutionDefaultCulture);
    }

    /// <summary>
    /// Returns string value of the intput using Solution/Customer default Culture Info.
    /// </summary>
    /// <param name="value">Input for conversion.</param>
    /// <returns>String value of input.</returns>
    public static string ToStringSolutionDefault(this int value)
    {
        return value.ToString(SolutionDefaultCulture);
    }

    /// <summary>
    /// Returns string value of the intput using Solution/Customer default Culture Info.
    /// </summary>
    /// <param name="value">Input for conversion.</param>
    /// <returns>String value of input.</returns>
    public static string ToStringSolutionDefault(this long value)
    {
        return value.ToString(SolutionDefaultCulture);
    }

    /// <summary>
    /// Returns string value of the intput using Solution/Customer default Culture Info.
    /// </summary>
    /// <param name="value">Input for conversion.</param>
    /// <returns>String value of input.</returns>
    public static string? ToLowerSolutionDefault(this string value)
    {
        return value?.ToLower(SolutionDefaultCulture);
    }
}
