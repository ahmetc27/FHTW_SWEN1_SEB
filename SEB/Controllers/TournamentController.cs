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

        TournamentResponse tournamentResponse = tournamentService.GetCurrentTournament(token);
        
        string message;
        object response;
        if(tournamentResponse.ActiveTournament != null)
        {
            message = "Active tournament retrieved successfully";
            response = new
            {
                message,
                tournament = tournamentResponse.ActiveTournament
            };
        }
        else if(tournamentResponse.PastTournaments != null && tournamentResponse.PastTournaments.Count > 0)
        {
            message = "No active tournament found. Listing past tournaments.";
            response = new
            {
                message,
                tournaments = tournamentResponse.PastTournaments
            };
        }
        else
        {
            message = ErrorMessages.TournamentNotFound;
            response = new
            {
                message
            };
        }
        
        Logger.Success(JsonSerializer.Serialize(response));
        Response.SendOk(writer, JsonSerializer.Serialize(response));
    }
}