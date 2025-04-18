namespace SEB.Server
{
    public class Response
    {
        public void SendResponse(StreamWriter writer, int statusCode, string statusMessage, string body)
        {
            writer.WriteLine($"HTTP/1.1 {statusCode} {statusMessage}");
            writer.WriteLine("Content-Type: application/json");
            writer.WriteLine();
            writer.WriteLine($"{body}");
        }

        public void SendOk(StreamWriter writer, string body)
        {
            SendResponse(writer, 200, "OK", $"{body}");
        }

        public void SendCreated(StreamWriter writer, string body)
        {
            SendResponse(writer, 201, "Created", $"{body}");
        }

        public void SendBadRequest(StreamWriter writer, string body)
        {
            SendResponse(writer, 400, "Bad Request", $"{body}");
        }

        public void SendUnauthorized(StreamWriter writer, string body)
        {
            SendResponse(writer, 401, "Unauthorized", $"{body}");
        }

        public void SendNotFound(StreamWriter writer, string body)
        {
            SendResponse(writer, 404, "Not Found", $"{body}");
        }

        public void SendConflict(StreamWriter writer, string body)
        {
            SendResponse(writer, 409, "Conflict", $"{body}");
        }

        public void SendInternalError(StreamWriter writer, string body)
        {
            SendResponse(writer, 500, "Internal Error", $"{body}");
        }
    }
}