using SEB.Models;
using SEB.Utils;
using SEB.Http;
using SEB.Interfaces;
using System.Text.Json;

namespace SEB.Controller;

public static class UserController
{
    public static void Register(StreamWriter writer, Request request, IUserService userService)
    {
        User? user = JsonSerializer.Deserialize<User>(request.Body)!;
        User? dbUser = userService.RegisterUser(user);

        var responseBody = new
        {
            message = "User created successfuly",
            user = dbUser
        };
        string json = JsonSerializer.Serialize(responseBody);

        Logger.Success($"User created: {json}");
        Response.SendCreated(writer, json);
    }
}