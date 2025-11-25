using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Aptiverse.Core.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// Converts a string to title case (capitalizes the first letter of each word)
        /// </summary>
        public static string ToTitleCase(this string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            var textInfo = CultureInfo.CurrentCulture.TextInfo;
            return textInfo.ToTitleCase(input.ToLower());
        }

        /// <summary>
        /// Truncates a string to the specified maximum length, adding an ellipsis if truncated
        /// </summary>
        public static string Truncate(this string value, int maxLength, string truncationSuffix = "...")
        {
            if (string.IsNullOrEmpty(value)) return value;
            return value.Length <= maxLength
                ? value
                : value.Substring(0, maxLength) + truncationSuffix;
        }

        /// <summary>
        /// Checks if the string contains the substring, using the specified comparison type
        /// </summary>
        public static bool Contains(this string source, string toCheck, StringComparison comp)
        {
            return source?.IndexOf(toCheck, comp) >= 0;
        }

        /// <summary>
        /// Converts a string to a boolean. Understands "true", "yes", "1", etc.
        /// </summary>
        public static bool ToBoolean(this string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return false;

            var trueValues = new[] { "true", "yes", "1", "y", "on" };
            var falseValues = new[] { "false", "no", "0", "n", "off" };

            var trimmed = value.Trim();
            if (trueValues.Contains(trimmed, StringComparer.OrdinalIgnoreCase))
                return true;
            if (falseValues.Contains(trimmed, StringComparer.OrdinalIgnoreCase))
                return false;

            throw new FormatException($"String '{value}' is not recognized as a valid boolean value.");
        }

        /// <summary>
        /// Removes all whitespace from a string
        /// </summary>
        public static string RemoveWhitespace(this string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            return new string(input.Where(c => !char.IsWhiteSpace(c)).ToArray());
        }

        /// <summary>
        /// Returns true if the string is null or empty
        /// </summary>
        public static bool IsNullOrEmpty(this string value)
        {
            return string.IsNullOrEmpty(value);
        }

        /// <summary>
        /// Returns true if the string is null, empty, or whitespace
        /// </summary>
        public static bool IsNullOrWhiteSpace(this string value)
        {
            return string.IsNullOrWhiteSpace(value);
        }

        /// <summary>
        /// Returns the string reversed
        /// </summary>
        public static string Reverse(this string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            char[] charArray = input.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }

        /// <summary>
        /// Converts a string to a secure SHA256 hash
        /// </summary>
        public static string ToSha256(this string input)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;

            using (var sha = System.Security.Cryptography.SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(input);
                var hash = sha.ComputeHash(bytes);
                return BitConverter.ToString(hash).Replace("-", "").ToLower();
            }
        }

        /// <summary>
        /// Extracts all digits from a string
        /// </summary>
        public static string ExtractDigits(this string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            return new string(input.Where(char.IsDigit).ToArray());
        }

        /// <summary>
        /// Converts a string to a URL-friendly slug
        /// </summary>
        public static string ToSlug(this string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            // Remove accents and special chars
            string normalized = input.Normalize(NormalizationForm.FormKD);
            normalized = Regex.Replace(normalized, @"\p{IsCombiningDiacriticalMarks}+", string.Empty);

            // Convert to lowercase and replace spaces
            string slug = normalized.ToLower()
                .Replace(" ", "-")
                .Replace(".", "-")
                .Replace(",", "-");

            // Remove invalid chars
            slug = Regex.Replace(slug, @"[^a-z0-9\-]", "");

            // Convert multiple dashes to single dash
            slug = Regex.Replace(slug, @"-+", "-");

            // Trim dashes from start and end
            return slug.Trim('-');
        }

        /// <summary>
        /// Counts the number of words in a string
        /// </summary>
        public static int WordCount(this string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return 0;

            return Regex.Split(input.Trim(), @"\s+").Length;
        }

        /// <summary>
        /// Checks if the string is a valid email address
        /// </summary>
        public static bool IsValidEmail(this string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return false;

            try
            {
                return Regex.IsMatch(input,
                    @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
                    RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }

        /// <summary>
        /// Converts a string to camelCase
        /// </summary>
        public static string ToCamelCase(this string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            var words = input.Split([' ', '_', '-'], StringSplitOptions.RemoveEmptyEntries);
            var firstWord = words[0].ToLower();
            var restWords = words.Skip(1)
                .Select(word => char.ToUpper(word[0]) + word.Substring(1).ToLower());

            return firstWord + string.Concat(restWords);
        }

        /// <summary>
        /// Converts a string to PascalCase
        /// </summary>
        public static string ToPascalCase(this string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            return string.Concat(input.Split(new[] { ' ', '_', '-' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(word => char.ToUpper(word[0]) + word.Substring(1).ToLower()));
        }

        /// <summary>
        /// Masks part of a string for privacy (e.g., "1234567890" becomes "*******7890")
        /// </summary>
        public static string Mask(this string input, int visibleChars = 4, char maskChar = '*')
        {
            if (string.IsNullOrEmpty(input) || input.Length <= visibleChars)
                return input;

            return new string(maskChar, input.Length - visibleChars) + input.Substring(input.Length - visibleChars);
        }
    }
}
