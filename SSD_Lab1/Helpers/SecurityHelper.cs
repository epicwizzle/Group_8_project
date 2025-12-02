using System.IO;
using System.Text.Encodings.Web;
using System.Text.RegularExpressions;

namespace SSD_Lab1.Helpers
{
    public static class SecurityHelper
    {
        /// <summary>
        /// Sanitizes a string for use in HTML attributes to prevent XSS attacks
        /// </summary>
        /// <param name="input">The input string to sanitize</param>
        /// <returns>Sanitized string safe for HTML attributes</returns>
        public static string SanitizeAttribute(string? input)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;

            // Remove any HTML tags
            input = Regex.Replace(input, "<.*?>", string.Empty);
            
            // HTML encode the input
            input = HtmlEncoder.Default.Encode(input);
            
            // Remove any remaining dangerous characters
            input = Regex.Replace(input, @"[^\w\s\-\.]", string.Empty);
            
            return input.Trim();
        }

        /// <summary>
        /// Validates and sanitizes an ID value for use in route parameters
        /// </summary>
        /// <param name="id">The ID value to validate</param>
        /// <returns>Sanitized ID or empty string if invalid</returns>
        public static string SanitizeId(object? id)
        {
            if (id == null)
                return string.Empty;

            string idString = id.ToString() ?? string.Empty;
            
            // Only allow alphanumeric characters and hyphens
            if (Regex.IsMatch(idString, @"^[a-zA-Z0-9\-]+$"))
            {
                return idString;
            }
            
            return string.Empty;
        }

        /// <summary>
        /// Validates an integer ID to ensure it's safe
        /// </summary>
        /// <param name="id">The ID to validate</param>
        /// <returns>True if the ID is valid and safe</returns>
        public static bool IsValidId(int? id)
        {
            return id.HasValue && id.Value > 0;
        }

        /// <summary>
        /// Validates and sanitizes a file path to prevent path traversal attacks
        /// </summary>
        /// <param name="path">The file path to validate</param>
        /// <param name="baseDirectory">The base directory that paths must be within</param>
        /// <returns>Sanitized path or empty string if invalid</returns>
        public static string SanitizePath(string? path, string baseDirectory)
        {
            if (string.IsNullOrEmpty(path))
                return string.Empty;

            // Remove any path traversal sequences
            path = path.Replace("..", string.Empty);
            path = path.Replace("//", "/");
            path = path.Replace("\\\\", "\\");

            // Remove any leading slashes or backslashes
            path = path.TrimStart('/', '\\');

            // Combine with base directory and get full path
            try
            {
                var fullPath = Path.GetFullPath(Path.Combine(baseDirectory, path));
                var baseFullPath = Path.GetFullPath(baseDirectory);

                // Ensure the resolved path is within the base directory
                if (!fullPath.StartsWith(baseFullPath, StringComparison.OrdinalIgnoreCase))
                {
                    return string.Empty;
                }

                return fullPath;
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Validates input to prevent SQL injection by checking for dangerous SQL keywords
        /// </summary>
        /// <param name="input">The input string to validate</param>
        /// <returns>True if input appears safe, false if potentially dangerous</returns>
        public static bool IsSafeFromSqlInjection(string? input)
        {
            if (string.IsNullOrEmpty(input))
                return true;

            // List of dangerous SQL keywords and patterns
            var dangerousPatterns = new[]
            {
                "';", "--", "/*", "*/", "xp_", "sp_", "exec", "execute", "union", "select", "insert", "update",
                "delete", "drop", "create", "alter", "truncate", "declare", "cast", "convert", "script",
                "'1'='1", "'1'='1'", "1'='1", "1'='1'", "or '1'='1", "or 1=1", "or 1=1--", "or '1'='1'--"
            };
            
            // Check for single quotes which are used in SQL injection
            if (input.Contains("'") && (input.Contains("=") || input.Contains("or") || input.Contains("and")))
            {
                return false;
            }

            var lowerInput = input.ToLowerInvariant();

            foreach (var pattern in dangerousPatterns)
            {
                if (lowerInput.Contains(pattern))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Sanitizes input to prevent SQL injection
        /// </summary>
        /// <param name="input">The input string to sanitize</param>
        /// <returns>Sanitized string safe from SQL injection</returns>
        public static string SanitizeSqlInput(string? input)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;

            // Remove SQL comment patterns
            input = Regex.Replace(input, "--.*", string.Empty, RegexOptions.IgnoreCase);
            input = Regex.Replace(input, @"/\*.*?\*/", string.Empty, RegexOptions.IgnoreCase | RegexOptions.Singleline);

            // Remove semicolons that could terminate statements
            input = input.Replace(";", string.Empty);

            // HTML encode to prevent script injection
            input = HtmlEncoder.Default.Encode(input);

            return input.Trim();
        }
    }
}

