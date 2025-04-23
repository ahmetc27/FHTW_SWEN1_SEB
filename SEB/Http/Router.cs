using SEB.Utils;
using SEB.Controller;
using SEB.Interfaces;

namespace SEB.Http;
public class Router
{
    public IUserService _userService;
    public Router(IUserService userService)
    {
        _userService = userService;
    }

    public void Route(Request request, StreamWriter writer)
    {
        try
        {
            switch (request.Method)
            {
                case "POST":
                    //users
                    if (request.Path == "/users")
                    {
                        UserController.Register(writer, request, _userService);
                    }

                    //sessions
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