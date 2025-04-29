using SEB.Exceptions;
using SEB.Interfaces;
using SEB.DTOs;
using SEB.Utils;

namespace SEB.Services;

public class StatsService : IStatsService
{
    private readonly ISessionRepository sessionRepository;
    private readonly IStatsRepository statsRepository;
    public StatsService(ISessionRepository sessionRepository, IStatsRepository statsRepository)
    {
        this.sessionRepository = sessionRepository;        
        this.statsRepository = statsRepository;
    }
    public Stats GetStatistics(string token)
    {
        RequestHelper.ValidateCredentials(token, "Token");

        if(!sessionRepository.ExistToken(token))
            throw new UnauthorizedException(ErrorMessages.TokenNotFound);

        var userData = statsRepository.GetUserStatsByToken(token)
            ?? throw new BadRequestException(ErrorMessages.StatsNotFound);

        var rank = GetRank(userData.elo);

        return new Stats
        {
            Id = userData.userId,
            Elo = userData.elo,
            OverallPushups = userData.totalPushups,
            Rank = rank
        };
    }

    public List<Stats> GetAllStatistics(string token)
    {
        RequestHelper.ValidateCredentials(token, "Token");

        if(!sessionRepository.ExistToken(token))
            throw new UnauthorizedException(ErrorMessages.TokenNotFound);

        var stats = statsRepository.GetAllStats();

        foreach(var stat in stats)
            stat.Rank = GetRank(stat.Elo);

        return stats;
    }

    private string GetRank(int elo)
    {
        if(elo <= 90)
            return "Bronze";

        if(elo <= 100)
            return "Silver";

        if(elo <= 110)
            return "Gold";

        return "Diamond";
    }
}