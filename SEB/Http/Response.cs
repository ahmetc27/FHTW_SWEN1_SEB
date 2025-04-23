namespace SEB.Http;

public static class Response
{
    private static void SendResponse(StreamWriter writer, int statusCode, string message, string body, string contentType = "application/json")
    {
        writer.WriteLine($"HTTP/1.1 {statusCode} {message}");
        writer.WriteLine($"Content-Type: {contentType}");
        writer.WriteLine();
        writer.WriteLine(body);
    }

    public static void SendOk(StreamWriter writer, string body)
    {
        SendResponse(writer, 200, "OK", body);
    }

    public static void SendCreated(StreamWriter writer, string body)
    {
        SendResponse(writer , 201, "Created", body);
    }

    public static void SendBadRequest(StreamWriter writer, string body)
    {
        SendResponse(writer, 400, "Bad Request", body);
    }

    public static void SendUnauthorized(StreamWriter writer, string body)
    {
        SendResponse(writer, 401, "Unauthorized", body);
    }

    public static void SendNotFound(StreamWriter writer, string body)
    {
        SendResponse(writer, 404, "Not Found", body);
    }

    public static void SendInternalServerError(StreamWriter writer, string body)
    {
        SendResponse(writer, 500, "Internal Server Error", body);
    }
}