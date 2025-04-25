namespace SEB.DTOs;

public class HistoryRequest
{
    public string Name { get; set; } = string.Empty;
    public int Count { get; set; }
    public int DurationInSeconds { get; set; }
}