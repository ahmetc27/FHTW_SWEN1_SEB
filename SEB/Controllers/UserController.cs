using SEB.Models;
using SEB.DTOs;
using SEB.Utils;
using SEB.Http;
using SEB.Interfaces;
using System.Text.Json;
using SEB.Exceptions;
using SEB.Mappers;

namespace SEB.Controller;

public static class UserController
{
    public static void Register(StreamWriter writer, Request request, IUserService userService)
    {
        UserCredentials? userCreds = JsonSerializer.Deserialize<UserCredentials>(request.Body)
            ?? throw new BadRequestException("Invalid JSON body");

        User createdUser = userService.RegisterUser(userCreds);

        var responseBody = new
        {
            message = "User created successfully",
            user = UserMapper.ToUserResponse(createdUser)
        };
        string json = JsonSerializer.Serialize(responseBody);

        Logger.Success($"User created: {json}");
        Response.SendCreated(writer, json);
    }

    public static void GetUserProfile(StreamWriter writer, Request request, IUserService userService)
    {
        string username = RequestHelper.GetUsernameFromRequest(request) 
            ?? throw new BadRequestException("Username invalid in request line");
        
        string token = RequestHelper.GetAuthToken(request) 
            ?? throw new UnauthorizedException("Invalid token");
        
        User dbUser = userService.ValidateUserAccess(username, token);       

        var responseBody = new
        {
            message = "User profile retrieved successfully",
            user = UserMapper.ToUserResponse(dbUser)
        };
        string json = JsonSerializer.Serialize(responseBody);

        Logger.Success($"User profile retrieved successfully: {json}");
        Response.SendOk(writer, json);
    }

    public static void UpdateUserProfile(StreamWriter writer, Request request, IUserService userService)
    {
        string username = RequestHelper.GetUsernameFromRequest(request)
            ?? throw new BadRequestException("Username invalid in request line");
        
        string token = RequestHelper.GetAuthToken(request)
            ?? throw new UnauthorizedException("Invalid token");

        User? dbUser = userService.ValidateUserAccess(username, token)!;

        UserProfile? requestUserProfile = JsonSerializer.Deserialize<UserProfile>(request.Body)
            ?? throw new BadRequestException("Invalid JSON body");

        userService.CheckUserProfile(requestUserProfile, dbUser);

        var responseBody = new
        {
            message = "User profile updated successfully",
            user = UserMapper.ToUserResponse(dbUser)
        };
        string json = JsonSerializer.Serialize(responseBody);

        Logger.Success($"User profile updated: {json}");
        Response.SendOk(writer, json);
    }
}