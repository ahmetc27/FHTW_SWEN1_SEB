using SEB.Models;

namespace SEB.Interfaces;

public interface IStatsRepository
{
    public (int userId, int elo, int totalPushups)? GetUserStatsByToken(string token);
    public List<Stats> GetAllStats();
}