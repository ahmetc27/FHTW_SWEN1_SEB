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

        return new Stats
        {
            Id = userData.userId,
            Elo = userData.elo,
            OverallPushups = userData.totalPushups
        };
    }

    public List<Stats> GetAllStatistics(string token)
    {
        RequestHelper.ValidateCredentials(token, "Token");

        if(!sessionRepository.ExistToken(token))
            throw new UnauthorizedException(ErrorMessages.TokenNotFound);

        return statsRepository.GetAllStats();
    }
}