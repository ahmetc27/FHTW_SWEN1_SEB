using SEB.Models;

namespace SEB.Interfaces;

public interface IStatsService
{
    public Stats GetUserStatistics(string token);
}