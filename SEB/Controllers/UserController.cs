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
        userService.RegisterUser(user);
        string json = JsonSerializer.Serialize(user);
        Logger.Success($"User successfuly created: {json}");
        Response.SendCreated(writer, json);
    }
}