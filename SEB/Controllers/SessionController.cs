using System.Text.Json;
using SEB.Exceptions;
using SEB.Http;
using SEB.Interfaces;
using SEB.Models;
using SEB.Utils;

namespace SEB.Controller;

public static class SessionController
{
    public static void Login(StreamWriter writer, Request request, IUserService userService, ISessionService sessionService)
    {
        UserCredentials? userCreds = JsonSerializer.Deserialize<UserCredentials>(request.Body);

        if(userCreds == null)
        {
            Logger.Error("Invalid JSON body");
            throw new BadRequestException("Invalid JSON body");
        }

        User dbUser = userService.AuthenticateUser(userCreds);

        sessionService.CreateToken(dbUser);

        var responseBody = new
        {
            message = "Session created successfully",
            user = new UserResponse
            {
                Id = dbUser.UserId,
                Username = dbUser.Username,
                Elo = dbUser.Elo,
                Name = dbUser.Name,
                Bio = dbUser.Bio,
                Image = dbUser.Image
            }
        };
        string json = JsonSerializer.Serialize(responseBody);
        
        Logger.Success($"Session created successfully: {json}");
        Response.SendCreated(writer, json);
    }
}