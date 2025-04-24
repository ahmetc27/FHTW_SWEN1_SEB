using System.Collections;
using SEB.Models;

namespace SEB.Interfaces;

public interface IStatsService
{
    public Stats GetStatistics(string token);
    public List<Stats> GetAllStatistics(string token);
}