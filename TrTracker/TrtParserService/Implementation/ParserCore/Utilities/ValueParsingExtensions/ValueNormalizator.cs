namespace TrtParserService.Implementation.ParserCore.Utilities.ValueParsingExtensions
{
    public static class ValueNormalizator
    {
        public static string? ToUtcIso(this DateTimeOffset? dto)
            => dto?.ToUniversalTime().ToString("o");

        public static string? NormalizeOutcome(this string? s)
        {
            if (string.IsNullOrWhiteSpace(s)) 
                return null;

            switch (s.Trim().ToLowerInvariant())
            {
                case "pass":
                case "passed":          // TRX
                case "success":
                    return "Passed";

                case "fail":
                case "failed":          // TRX
                    return "Failed";

                case "notexecuted":     // TRX
                case "skipped":
                case "ignored":
                case "inconclusive":    // TRX
                case "pending":
                case "notrunnable":     // TRX
                    return "Skipped";

                case "error":           // TRX
                case "aborted":         // TRX
                case "timeout":         // TRX
                    return "Error";

                default: return s;
            }
        }
    }
}
