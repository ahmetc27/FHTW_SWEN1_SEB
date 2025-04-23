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
            message = "Session created successfully",
            user = dbUser
        };
        string json = JsonSerializer.Serialize(responseBody);
        
        Logger.Success($"Session created successfully: {json}");
        Response.SendCreated(writer, json);
    }
}