using SEB.Models;

namespace SEB.Interfaces;

public interface IStatsRepository
{
    public int? GetEloByToken(string token);
    public int? GetTotalPushupsById(int userId);
    public List<Stats> GetAllStats();

}