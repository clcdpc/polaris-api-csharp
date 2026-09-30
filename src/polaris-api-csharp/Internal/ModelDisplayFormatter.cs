using System.Globalization;

namespace Clc.Polaris.Api.Internal
{
    internal static class ModelDisplayFormatter
    {
        public static string MaskLeadingDigits(long value, int visibleTrailingDigits = 4)
        {
            return MaskLeadingCharacters(value.ToString(CultureInfo.InvariantCulture), visibleTrailingDigits);
        }

        public static string MaskLeadingCharacters(string value, int visibleTrailingDigits = 4)
        {
            ArgumentNullException.ThrowIfNull(value);

            if (visibleTrailingDigits < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(visibleTrailingDigits), visibleTrailingDigits, "Visible trailing digits cannot be negative.");
            }

            var hasSign = value.StartsWith("-", StringComparison.Ordinal);
            var digits = hasSign ? value[1..] : value;

            var visibleLength = Math.Min(visibleTrailingDigits, Math.Max(0, digits.Length - 1));
            var maskedLength = digits.Length - visibleLength;

            return string.Concat(
                hasSign ? "-" : string.Empty,
                new string('*', maskedLength),
                digits[^visibleLength..]);
        }
    }
}