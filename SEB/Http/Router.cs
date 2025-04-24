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
    public Router(IUserService userService, ISessionService sessionService, IStatsService statsService, IHistoryService historyService)
    {
        this.userService = userService;
        this.sessionService = sessionService;
        this.statsService = statsService;
        this.historyService = historyService;
    }

    public void Route(Request request, StreamWriter writer)
    {
        try
        {
            switch(request.Method)
            {
                case "POST":
                    //users
                    if(request.Path.StartsWith("/users"))
                    {
                        if(request.Path != "/users")
                            throw new BadRequestException("Invalid path. Expected POST /users");

                        UserController.Register(writer, request, userService);
                    }
                    //sessions
                    else if(request.Path.StartsWith("/sessions"))
                    {
                        if(request.Path != "/sessions")
                            throw new BadRequestException("Invalid path. Expected POST /sessions");

                        SessionController.Login(writer, request, userService, sessionService);
                    }
                    //history
                    // else if
                    else
                        throw new BadRequestException("Invalid path");

                    break;

                case "GET":
                    if(request.Path.StartsWith("/users")) //users/test
                        UserController.GetUserByName(writer, request, userService);

                    else if(request.Path.StartsWith("/stats")) //stats
                    {
                        if(request.Path != "/stats")
                            throw new BadRequestException("Invalid path. Expected GET /stats");

                        StatsController.GetStats(writer, request, statsService);
                    }
                    
                    else if(request.Path.StartsWith("/score")) //scoreboard
                    {
                        if(request.Path != "/score")
                            throw new BadRequestException("Invalid path. Expected GET /score");
                        
                        StatsController.GetAllStats(writer, request, statsService);
                    }

                    else if(request.Path.StartsWith("/history")) //history
                    {
                        if(request.Path != "/history")
                            throw new BadRequestException("Invalid path. Expected GET /history");
                        
                        HistoryController.GetHistory(writer, request, historyService);
                    }
                    else
                        throw new BadRequestException("Invalid path");
                    //tournament
                    break;

                case "PUT":
                    if(request.Path.StartsWith("/users")) //users/test
                        UserController.UpdateUserProfile(writer, request, userService);
                    else
                        throw new BadRequestException("Invalid path");
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
}