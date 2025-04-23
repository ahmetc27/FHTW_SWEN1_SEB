using System.Text.Json;
using SEB.Http;
using SEB.Interfaces;
using SEB.Models;
using SEB.Utils;

namespace SEB.Controller;

public static class SessionController
{
    public static void Login(StreamWriter writer, Request request, IUserService userService, ISessionService sessionService)
    {
        User? user = JsonSerializer.Deserialize<User>(request.Body)!;
        User? dbUser = userService.ValidateUser(user)!;
        sessionService.CreateToken(dbUser);

        var responseBody = new
        {
            message = "User created successfuly",
            user = dbUser
        };
        string json = JsonSerializer.Serialize(responseBody);
        
        Logger.Success($"Token successfully created: {json}");
        Response.SendCreated(writer, json);
    }
}