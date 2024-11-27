using System.Globalization;

namespace SharedDomain
{
    public static class CultureInfoHelperExtensions
    {
        private static readonly CultureInfo DefaultCulture = new CultureInfo("da-DK");

        /// <summary>
        ///  Returns string value of the intput using LF's default Culture Info.
        /// </summary>
        /// <param name="value">Input for conversion.</param>
        /// <returns>String value of input.</returns>
        public static string ToStringLfDefault(this DateTime value)
        {
            return value.ToString(DefaultCulture);
        }

        /// <summary>
        /// Returns string value of the intput using LF's default Culture Info.
        /// </summary>
        /// <param name="value">Input for conversion.</param>
        /// <returns>String value of input.</returns>
        public static string ToStringLfDefault(this int value)
        {
            return value.ToString(DefaultCulture);
        }

        /// <summary>
        /// Returns string value of the intput using LF's default Culture Info.
        /// </summary>
        /// <param name="value">Input for conversion.</param>
        /// <returns>String value of input.</returns>
        public static string ToStringLfDefault(this long value)
        {
            return value.ToString(DefaultCulture);
        }

        /// <summary>
        /// Returns string value of the intput using LF's default Culture Info.
        /// </summary>
        /// <param name="value">Input for conversion.</param>
        /// <returns>String value of input.</returns>
        public static string? ToLowerLfDefault(this string value)
        {
            return value?.ToLower(DefaultCulture);
        }
    }
}