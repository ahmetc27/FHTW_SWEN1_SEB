namespace SEB.DTOs;
public class Stats
{
    public int Id { get; set; }
    public int Elo { get; set; }
    public int OverallPushups { get; set; }
    public string Rank { get; set; } = "Silver"; // silver, gold, diamond
}