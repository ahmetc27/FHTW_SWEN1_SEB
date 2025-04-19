namespace SEB.Models
{
    public class Participant
    {
        public int UserId { get; set; }
        public string Username { get; set; } = "";
        public int TotalPushups { get; set; }
    }
}