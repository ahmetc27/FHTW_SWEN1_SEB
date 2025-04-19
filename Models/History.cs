namespace SEB.Models
{
    public class History
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int PushupCount { get; set; }
        public int Duration { get; set; }
        public DateTime Timestamp { get; set; }
    }
}