using SEB.Exceptions;
using SEB.Http;

namespace SEB.Utils;

public static class RequestHelper
{
    public static string GetUsernameFromRequest(Request request)
    {
        string? username = request.Path.Split("/").LastOrDefault();
        
        if(string.IsNullOrWhiteSpace(username))
            throw new BadRequestException(ErrorMessages.InvalidUsername);
        
        return username;
    }

    public static string GetAuthToken(Request request)
    {
        if(!request.Headers.ContainsKey("Authorization"))
            throw new UnauthorizedException(ErrorMessages.TokenRequired);

        string? header = request.Headers["Authorization"];
        string? token = header?.Split(" ").LastOrDefault();

        if(string.IsNullOrWhiteSpace(token))
            throw new UnauthorizedException(ErrorMessages.InvalidToken);

        return token;
    }

    public static void ValidateCredentials(string value, string fieldName)
    {
        if(string.IsNullOrWhiteSpace(value))
            throw new BadRequestException($"{fieldName} invalid");
    }
}