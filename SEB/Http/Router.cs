using SEB.Utils;
using SEB.Controller;
using SEB.Interfaces;

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
                    Response.SendNotFound(writer, "Endpoint not found");
                    break;
            }
        }
        catch(Exception ex)
        {
            Logger.Error(ex.Message);
            Response.SendBadRequest(writer, ex.Message);
        }
    }
}