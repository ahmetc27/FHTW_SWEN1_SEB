using SEB.Utils;
using SEB.Controller;
using SEB.Interfaces;
using SEB.Exceptions;
using System.Text.Json;

namespace SEB.Http;
public class Router
{
    private readonly IUserService userService;
    private readonly ISessionService sessionService;
    private readonly IStatsService statsService;
    private readonly IHistoryService historyService;
    private readonly ITournamentService tournamentService;
    public Router(IUserService userService, ISessionService sessionService, IStatsService statsService, IHistoryService historyService, ITournamentService tournamentService)
    {
        this.userService = userService;
        this.sessionService = sessionService;
        this.statsService = statsService;
        this.historyService = historyService;
        this.tournamentService = tournamentService;
    }

    public void Route(Request request, StreamWriter writer)
    {
        try
        {
            switch(request.Method)
            {
                case "POST":
                    HandlePost(request, writer);
                    break;

                case "GET":
                    HandleGet(request, writer);
                    break;

                case "PUT":
                    HandlePut(request, writer);
                    break;

                default:
                    Logger.Error("Endpoint not found");
                    throw new BadRequestException("Endpoint not found");
            }
        }
        catch(BadRequestException ex)
        {
            Logger.Error($"Bad request error: {ex.Message}");
            Response.SendBadRequest(writer, ex.Message);
        }
        catch(UnauthorizedException ex)
        {
            Logger.Error($"Unauthorized error: {ex.Message}");
            Response.SendUnauthorized(writer, ex.Message);
        }
        catch(NotFoundException ex)
        {
            Logger.Error($"Not found error: {ex.Message}");
            Response.SendNotFound(writer, ex.Message);
        }
        catch(ConflictException ex)
        {
            Logger.Error($"Conflict error: {ex.Message}");
            Response.SendNotFound(writer, ex.Message);
        }
        catch(JsonException ex)
        {
            Logger.Error("Invalid JSON: " + ex.Message);
            Response.SendBadRequest(writer, "Invalid JSON format");
        }
        catch(Exception ex)
        {
            Logger.Error($"An error ocurred: {ex.Message}");
            Response.SendInternalServerError(writer, "An unexpected error occurred");
        }
    }

    private void HandlePost(Request request, StreamWriter writer)
    {        
        if(request.Path == "/users") //users
            UserController.Register(writer, request, userService);
        
        else if(request.Path == "/sessions") //sessions
            SessionController.Login(writer, request, userService, sessionService);
        
        else if(request.Path == "/history") //history
            HistoryController.LogPushups(writer, request, historyService);

        else
            throw new BadRequestException("Invalid path");
    }

    private void HandleGet(Request request, StreamWriter writer)
    {
        if(request.Path.StartsWith("/users")) //users/test
            UserController.GetUserProfile(writer, request, userService);

        else if(request.Path == "/stats") //stats
            StatsController.GetStats(writer, request, statsService);
        
        else if(request.Path == "/score") //scoreboard
            StatsController.GetAllStats(writer, request, statsService);

        else if(request.Path == "/history") //history
            HistoryController.GetHistory(writer, request, historyService);
        
        else if(request.Path == "/tournament") //tournament
            TournamentController.GetCurrentTournament(writer, request, tournamentService);
            
        else
            throw new BadRequestException("Invalid path");
    }

    private void HandlePut(Request request, StreamWriter writer)
    {
        if(request.Path.StartsWith("/users")) //users/test
            UserController.UpdateUserProfile(writer, request, userService);

        else
            throw new BadRequestException("Invalid path");
    }
}