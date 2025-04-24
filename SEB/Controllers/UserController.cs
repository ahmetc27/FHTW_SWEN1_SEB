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
        // 1. Validate HTTP-level inputs (fail fast)
        UserCredentials? userCreds = JsonSerializer.Deserialize<UserCredentials>(request.Body)
            ?? throw new BadRequestException(ErrorMessages.InvalidJson);

        // 2. Let the service handle the rest (business logic + persistence)
        User createdUser = userService.RegisterUser(userCreds);

        // 3. Return response
        var responseBody = new
        {
            message = "User created successfully",
            user = UserMapper.ToUserResponse(createdUser)
        };

        Logger.Success($"User created: {JsonSerializer.Serialize(responseBody)}");
        Response.SendCreated(writer, JsonSerializer.Serialize(responseBody));
    }

    public static void GetUserProfile(StreamWriter writer, Request request, IUserService userService)
    {
        // 1. Validate HTTP-level inputs (fail fast)
        string username = RequestHelper.GetUsernameFromRequest(request) 
            ?? throw new BadRequestException(ErrorMessages.InvalidUsername);
        
        string token = RequestHelper.GetAuthToken(request) 
            ?? throw new UnauthorizedException(ErrorMessages.InvalidToken);
        
        // 2. Let the service handle the rest (business logic + persistence)
        User dbUser = userService.ValidateUserAccess(username, token);       

        // 3. Return response
        var responseBody = new
        {
            message = "User profile retrieved successfully",
            user = UserMapper.ToUserResponse(dbUser)
        };

        Logger.Success($"User profile retrieved successfully: {JsonSerializer.Serialize(responseBody)}");
        Response.SendOk(writer, JsonSerializer.Serialize(responseBody));
    }

    public static void UpdateUserProfile(StreamWriter writer, Request request, IUserService userService)
    {
        // 1. Validate HTTP-level inputs (fail fast)
        string username = RequestHelper.GetUsernameFromRequest(request)
            ?? throw new BadRequestException(ErrorMessages.InvalidUsername);
        
        string token = RequestHelper.GetAuthToken(request)
            ?? throw new UnauthorizedException(ErrorMessages.InvalidToken);

        // 2. Deserialize and validate request body
        UserProfile requestUserProfile = JsonSerializer.Deserialize<UserProfile>(request.Body)
            ?? throw new BadRequestException(ErrorMessages.InvalidJson);

        // 3. Let the service handle the rest (business logic + persistence)
        User updatedUser = userService.UpdateUserProfile(username, token, requestUserProfile);

        // 4. Return response
        var responseBody = new
        {
            message = "User profile updated successfully",
            user = UserMapper.ToUserResponse(updatedUser)
        };

        Logger.Success($"Profile updated: {JsonSerializer.Serialize(responseBody)}");
        Response.SendOk(writer, JsonSerializer.Serialize(responseBody));
    }
}