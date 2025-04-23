using System.Collections;
using SEB.Exceptions;
using SEB.Interfaces;
using SEB.Models;

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
    public Stats GetUserStatistics(string token)
    {
        if(string.IsNullOrWhiteSpace(token))
            throw new BadRequestException("Token is missing or empty");

        if(!sessionRepository.ExistToken(token))
            throw new UnauthorizedException("Token does not exist");

        int? elo = statsRepository.GetEloByToken(token);
        int? userId = userRepository.GetIdByToken(token);

        if(userId == null)
            throw new UnauthorizedException("Invalid token");

        int? overallPushups = statsRepository.GetTotalPushupsById(userId.Value);

        if(elo == null || overallPushups == null)
            throw new BadRequestException("Could not retrieve stats");

        Stats stats = new Stats()
        {
            Elo = elo,
            OverallPushups = overallPushups
        };
        return stats;
    }

    public List<Stats> GetAllStatistics(string token)
    {
        if(string.IsNullOrWhiteSpace(token))
            throw new BadRequestException("Token is missing or empty");

        if(!sessionRepository.ExistToken(token))
            throw new UnauthorizedException("Token does not exist");

        List<Stats> scoreboard = new List<Stats>();

        //statsRepository. get all user stats
        return scoreboard;
    }
}