using SEB.DTOs;
using SEB.Utils;
using SEB.Http;
using SEB.Interfaces;
using System.Text.Json;
using SEB.Exceptions;

namespace SEB.Controller;

public static class StatsController
{
    public static void GetStats(StreamWriter writer, Request request, IStatsService statsService)
    {
        string token = RequestHelper.GetAuthToken(request)
            ?? throw new UnauthorizedException(ErrorMessages.InvalidToken);
        
        Stats userStats = statsService.GetStatistics(token);

        var responseBody = new
        {
            message = "User stats retrieved successfully",
            stats = userStats
        };
        
        Logger.Success($"User stats retrieved: {JsonSerializer.Serialize(responseBody)}");
        Response.SendOk(writer, JsonSerializer.Serialize(responseBody));
    }

    public static void GetAllStats(StreamWriter writer, Request request, IStatsService statsService)
    {
        string token = RequestHelper.GetAuthToken(request)
            ?? throw new UnauthorizedException(ErrorMessages.InvalidToken);

        List<Stats> scoreboard = statsService.GetAllStatistics(token);

        var responseBody = new
        {
            message = "Scoreboard retrieved successfully",
            scoreboard
        };

        Logger.Success($"Scoreboard retrieved: {JsonSerializer.Serialize(responseBody)}");
        Response.SendOk(writer, JsonSerializer.Serialize(responseBody));
    }
}