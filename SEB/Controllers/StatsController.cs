using SEB.Models;
using SEB.Utils;
using SEB.Http;
using SEB.Interfaces;
using System.Text.Json;
using SEB.Exceptions;
using System.Net;

namespace SEB.Controller;

public static class StatsController
{
    public static void GetStats(StreamWriter writer, Request request, IStatsService statsService)
    {
        if(!request.Headers.ContainsKey("Authorization"))
            throw new UnauthorizedException("Header token required");
        string? token = AuthHelper.GetTokenFromHeader(request.Headers)!;

        Stats? userStats = statsService.GetUserStatistics(token);

        var responseBody = new
        {
            message = "User stats retrieved successfully",
            stats = userStats
        };
        
        string json = JsonSerializer.Serialize(responseBody);
        Logger.Success($"User stats retrieved: {json}");
        Response.SendOk(writer, json);        
    }
}