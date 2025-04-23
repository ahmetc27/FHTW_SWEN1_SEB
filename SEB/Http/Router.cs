using SEB.Utils;
using SEB.Controller;
using SEB.Interfaces;
using SEB.Exceptions;

namespace SEB.Http;
public class Router
{
    public IUserService userService;
    public ISessionService sessionService;
    public Router(IUserService userService, ISessionService sessionService)
    {
        this.userService = userService;
        this.sessionService = sessionService;
    }

    public void Route(Request request, StreamWriter writer)
    {
        try
        {
            switch(request.Method)
            {
                case "POST":
                    //users
                    if(request.Path == "/users")
                    {
                        UserController.Register(writer, request, userService);
                    }

                    //sessions
                    if(request.Path == "/sessions")
                    {
                        SessionController.Login(writer, request, userService, sessionService);
                    }

                    //history
                    break;

                case "GET":
                    //users/test
                    //stats
                    //scoreboard
                    //history
                    //tournament
                    break;

                case "PUT":
                    //users/test
                    break;

                default:
                    Logger.Error("Endpoint not found");
                    throw new NotFoundException("Endpoint not found");
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
        catch(Exception ex)
        {
            Logger.Error(ex.Message);
            Response.SendBadRequest(writer, ex.Message);
        }
    }
}