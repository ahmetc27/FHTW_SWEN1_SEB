using SEB.Exceptions;
using SEB.Http;

namespace SEB.Utils;

public static class RequestHelper
{
    public static string GetUsernameFromRequest(Request request)
    {
        string? username = request.Path.Split("/").LastOrDefault();
        
        if(string.IsNullOrWhiteSpace(username))
            throw new BadRequestException("Username invalid in request line");
        
        return username;
    }

    public static string GetAuthToken(Request request)
    {
        if(!request.Headers.ContainsKey("Authorization"))
            throw new UnauthorizedException("Header token required");

        string? header = request.Headers["Authorization"];
        string? token = header?.Split(" ").LastOrDefault();

        if(string.IsNullOrWhiteSpace(token))
            throw new UnauthorizedException("Invalid token");

        return token;
    }
}