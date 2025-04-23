using SEB.Utils;

public static class AuthHelper
{
    public static string? GetTokenFromHeader(Dictionary<string, string> header)
    {
        if(header.TryGetValue("Authorization", out string? key))
        {
            if(key.StartsWith("Basic "))
                return key.Replace("Basic ", "").Trim();
        }
        return null;
    }
}