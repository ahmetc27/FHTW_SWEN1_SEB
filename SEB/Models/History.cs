namespace SEB.Models;
public class History
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Count { get; set; }
    public int Duration { get; set; }
    public int TournamentId { get; set; }
}