using SEB.Models;
using SEB.Utils;
using SEB.Http;
using SEB.Interfaces;
using System.Text.Json;
using SEB.Exceptions;

namespace SEB.Controller;

public static class UserController
{
    public static void Register(StreamWriter writer, Request request, IUserService userService)
    {
        UserCredentials? userCreds = JsonSerializer.Deserialize<UserCredentials>(request.Body);

        if(userCreds == null)
        {
            Logger.Error("Invalid request Body");
            throw new BadRequestException("Invalid request body");
        }

        User createdUser = userService.RegisterUser(userCreds);

        var responseBody = new
        {
            message = "User created successfully",
            user = createdUser
        };
        string json = JsonSerializer.Serialize(responseBody);

        Logger.Success($"User created: {json}");
        Response.SendCreated(writer, json);
    }

    public static void GetUserByName(StreamWriter writer, Request request, IUserService userService)
    {
        string? username = GetUsernameFromRequest(request)!;

        if(!request.Headers.ContainsKey("Authorization")) throw new UnauthorizedException("Header token required");
        string? token = AuthHelper.GetTokenFromHeader(request.Headers)!;
        
        User? dbUser = userService.ValidateUserAccess(username, token);

        var responseBody = new
        {
            message = "User profile retrieved successfully",
            user = dbUser
        };
        string json = JsonSerializer.Serialize(responseBody);

        Logger.Success($"User profile retrieved successfully: {json}");
        Response.SendOk(writer, json);
    }

    public static void UpdateUserProfile(StreamWriter writer, Request request, IUserService userService)
    {
        string? username = GetUsernameFromRequest(request)!;

        if(!request.Headers.ContainsKey("Authorization")) throw new UnauthorizedException("Header token required");
        string? token = AuthHelper.GetTokenFromHeader(request.Headers)!;

        User? dbUser = userService.ValidateUserAccess(username, token)!;

        UserProfile? requestUserProfile = JsonSerializer.Deserialize<UserProfile>(request.Body)!;

        userService.CheckUserProfile(requestUserProfile, dbUser);

        var responseBody = new
        {
            message = "User profile updated successfully",
            user = dbUser
        };
        string json = JsonSerializer.Serialize(responseBody);

        Logger.Success($"User profile updated: {json}");
        Response.SendOk(writer, json);
    }

    private static string? GetUsernameFromRequest(Request request)
    {
        string[] url = request.Path.Split('/');
        if(url[1] != "users") throw new BadRequestException("Invalid path. Expected /users/{username}");
        return url[2];
    }
}