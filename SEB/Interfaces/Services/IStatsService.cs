using System.Collections;
using SEB.DTOs;

namespace SEB.Interfaces;

public interface IStatsService
{
    public Stats GetStatistics(string token);
    public List<Stats> GetAllStatistics(string token);
}