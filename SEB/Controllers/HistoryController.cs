using System.Text.Json;
using SEB.Exceptions;
using SEB.Http;
using SEB.Interfaces;
using SEB.Models;
using SEB.Utils;

namespace SEB.Controller;

public static class HistoryController
{
    public static void GetHistory(StreamWriter writer, Request request, IHistoryService historyService)
    {
        string? token = RequestHelper.GetAuthToken(request)
            ?? throw new UnauthorizedException("Invalid token");

        History history = historyService.GetUserHistoryData(token);

        var responseBody = new
        {
            message = "History retrieved successfully",
            history
        };
        string json = JsonSerializer.Serialize(responseBody);
        
        Logger.Success($"History retrieved successfully: {json}");
        Response.SendCreated(writer, json);
    }

    public static void LogPushups(StreamWriter writer, Request request, IHistoryService historyService)
    {
        string? token = RequestHelper.GetAuthToken(request)
            ?? throw new UnauthorizedException("Invalid token");
    }
}