using System.Collections;
using SEB.Exceptions;
using SEB.Interfaces;
using SEB.Models;
using SEB.Utils;

namespace SEB.Services;

public class StatsService : IStatsService
{
    private readonly IUserRepository userRepository;
    private readonly ISessionRepository sessionRepository;
    private readonly IStatsRepository statsRepository;
    public StatsService(IUserRepository userRepository, ISessionRepository sessionRepository, IStatsRepository statsRepository)
    {
        this.userRepository = userRepository;
        this.sessionRepository = sessionRepository;        
        this.statsRepository = statsRepository;
    }
    public Stats GetStatistics(string token)
    {
        RequestHelper.ValidateCredentials(token, "Token");

        if(!sessionRepository.ExistToken(token))
            throw new UnauthorizedException("Token does not exist");

        var userData = statsRepository.GetUserStatsByToken(token)
            ?? throw new BadRequestException("Could not retrieve user stats");

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
            throw new UnauthorizedException("Token does not exist");

        return statsRepository.GetAllStats();
    }
}