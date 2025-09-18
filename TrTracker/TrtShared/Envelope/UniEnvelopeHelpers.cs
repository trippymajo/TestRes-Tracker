namespace TrtShared.Envelope
{
    /// <summary>
    /// Helper class for retreiving data from UniEnvelope
    /// </summary>
    public static class UniEnvelopeHelpers
    {
        public static string? GetString(Dictionary<string, object?> data, string key) =>
            data.TryGetValue(key, out var v) ? v as string : null;

        public static long? GetLong(Dictionary<string, object?> data, string key) =>
            data.TryGetValue(key, out var v) && v != null ? Convert.ToInt64(v) : null;

        public static int? GetInt(Dictionary<string, object?> data, string key) =>
            data.TryGetValue(key, out var v) && v != null ? Convert.ToInt32(v) : null;

        public static DateTimeOffset? GetDate(Dictionary<string, object?> data, string key)
        {
            if (!data.TryGetValue(key, out var v) || v is null)
                return null;

            if (v is string s && DateTimeOffset.TryParse(s, out var dto))
                return dto;

            return null;
        }

        public static Dictionary<string, object?>? GetDict(Dictionary<string, object?> data, string key) =>
            data.TryGetValue(key, out var v) ? v as Dictionary<string, object?> : null;

        public static List<Dictionary<string, object?>>? GetList(Dictionary<string, object?> data, string key) =>
            data.TryGetValue(key, out var v) ? v as List<Dictionary<string, object?>> : null;

        public static bool TryGet(Dictionary<string, object?> data, string key, out object? val) =>
            data.TryGetValue(key, out val);
    }
}
