using System.Text.Json;
using SEB.DTOs;
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
        string token = RequestHelper.GetAuthToken(request)
            ?? throw new UnauthorizedException(ErrorMessages.InvalidToken);

        History history = historyService.GetUserHistoryData(token);

        var responseBody = new
        {
            message = "History retrieved successfully",
            count = history.Count,
            duration = history.Duration
        };
        
        Logger.Success($"History retrieved successfully: {JsonSerializer.Serialize(responseBody)}");
        Response.SendOk(writer, JsonSerializer.Serialize(responseBody));
    }

    public static void LogPushups(StreamWriter writer, Request request, IHistoryService historyService)
    {
        string token = RequestHelper.GetAuthToken(request)
            ?? throw new UnauthorizedException(ErrorMessages.InvalidToken);

        HistoryRequest historyRequest = JsonSerializer.Deserialize<HistoryRequest>(request.Body)
            ?? throw new BadRequestException(ErrorMessages.InvalidRequestBody);
        
        History addedHistory = historyService.LogPushups(token, historyRequest);        

        var responseBody = new
        {
            message = "Pushups logged successfully",
            addedHistory
        };

        Logger.Success($"Pushups logged successfully: {JsonSerializer.Serialize(responseBody)}");
        Response.SendCreated(writer, JsonSerializer.Serialize(responseBody));
    }
}