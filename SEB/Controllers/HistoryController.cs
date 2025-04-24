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
        if(!request.Headers.ContainsKey("Authorization"))
            throw new UnauthorizedException("Header token required");
        string? token = AuthHelper.GetTokenFromHeader(request.Headers)!;

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
}