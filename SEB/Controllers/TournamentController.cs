using System.Text.Json;
using SEB.Exceptions;
using SEB.Http;
using SEB.Interfaces;
using SEB.Models;
using SEB.Utils;

namespace SEB.Controller;

public static class TournamentController
{
    public static void GetCurrentTournament(StreamWriter writer, Request request, ITournamentService tournamentService)
    {
        string token = RequestHelper.GetAuthToken(request)
            ?? throw new UnauthorizedException(ErrorMessages.InvalidToken);

        Tournament? tournament = tournamentService.GetCurrentTournament(token);

        var message = tournament == null 
            ? "No tournament found" 
            : tournament.Status == "active"
                ? "Active tournament retrieved successfully" 
                : "Ended tournament retrieved successfully";
        
        var response = new
        {
            message,
            tournament
        };
        
        Logger.Success(JsonSerializer.Serialize(response));
        Response.SendOk(writer, JsonSerializer.Serialize(response));
    }
}