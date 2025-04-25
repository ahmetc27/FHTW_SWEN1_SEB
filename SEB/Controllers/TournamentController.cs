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

        Tournament tournament = tournamentService.GetCurrentTournament(token)
            ?? throw new BadRequestException(ErrorMessages.TournamentNotFound);

        var response = new
        {
            message = "Tournament infos retrieved successfully",
            tournament
        };
        Logger.Success(JsonSerializer.Serialize(response));
        Response.SendOk(writer, JsonSerializer.Serialize(response));
    }
}