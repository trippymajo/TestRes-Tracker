namespace TrtShared.DTO
{
    public class TestRunDTO
    {
        public string Branch { get; set; } = string.Empty;
        public string Version { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public List<ResultDTO> Results { get; set; } = new();
    }
}
