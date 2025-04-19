namespace SEB.Models
{
    public class Tournament
    {
        public int Id { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string Status { get; set; } = "in_progress"; // in_progress, completed
        public int? WinnerId { get; set; }
        public bool IsDraw { get; set; } = false;
        public List<Participant> Participants { get; set; } = new();
    }
}