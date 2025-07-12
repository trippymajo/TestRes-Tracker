namespace TrtShared.Envelope
{
    /// <summary>
    /// Universal data transfering format as JSON
    /// </summary>
    public class UniEnvelope
    {
        public string SchemaId { get; set; } = string.Empty;
        public Dictionary<string, object?> Data { get; set; } = new();
    }
}
