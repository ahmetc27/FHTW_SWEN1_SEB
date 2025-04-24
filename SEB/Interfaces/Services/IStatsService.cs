using System.Collections;
using SEB.Models;

namespace SEB.Interfaces;

public interface IStatsService
{
    public Stats GetUserStatistics(string token);
    public List<Stats> GetAllStatistics(string token);
}