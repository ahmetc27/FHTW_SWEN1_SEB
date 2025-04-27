namespace SEB.Models;

public class Tournament
{
    public int Id { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public string Status { get; set; } = "active"; // "active" or "ended"
    public string? Winner { get; set; }
}