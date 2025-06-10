using System.Text.RegularExpressions;

namespace Samqtt.Common
{
    public static partial class SanitizeHelpers
    {
        [GeneratedRegex("[^a-z0-9_]+")]
        private static partial Regex SanitizationRegex();

        public static string Sanitize(string value) => SanitizationRegex().Replace(value.ToLowerInvariant(), "_");
    }
}
