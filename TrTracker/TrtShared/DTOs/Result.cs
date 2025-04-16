namespace TrtShared.DTO
{
    public class ResultDTO
    {
        public int Id { get; set; }
        public string Outcome { get; set; } = string.Empty;
        public string? ErrMsg { get; set; }
    }
}
