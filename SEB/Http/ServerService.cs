using SEB.Utils;
using System.Text;
using SEB.Interfaces;
using SEB.Exceptions;

namespace SEB.Http;
public class ServerService : IServerService
{
    private int contentLength;
    public void ParseRequest(StreamReader reader, Request request)
    {
        ParseHeaders(reader, request);
        if(RequestHasBody(request)) ParseBody(reader, request);
    }
    public void ParseHeaders(StreamReader reader, Request request)
    {
        string? line;
        line = reader.ReadLine();
        if(line == null) throw new BadRequestException("Unexpected end of request");
        string[] parts = line.Split(' ');
        request.Method = parts[0];
        request.Path = parts[1];
        request.Version = parts[2];
        Logger.Info($"Method: {request.Method}, Path: {request.Path}");

        while(!string.IsNullOrEmpty(line = reader.ReadLine()))
        {
            string[] headerParts = line.Split(":", 2);
            if(headerParts.Length == 2)
            {
                string key = headerParts[0];
                string value = headerParts[1].Trim();
                
                request.Headers.Add(key, value);

                if(key == "Content-Length")
                {
                    contentLength = int.Parse(value);
                }
                
                Logger.Info($"Header: {key}: {value}");
            }
        }
    }

    public void ParseBody(StreamReader reader, Request request)
    {
        if(contentLength > 0)
        {
            StringBuilder sb = new StringBuilder();
            char[] buffer = new char[1024];
            int bytesRead;
            int bytesReadTotal = 0;

            while(bytesReadTotal < contentLength)
            {
                bytesRead = reader.Read(buffer, 0, buffer.Length);

                if(bytesRead == 0) break;

                bytesReadTotal += bytesRead;
                sb.Append(buffer, 0, bytesRead);
            }
            request.Body = sb.ToString();
            
        }
        Logger.Info($"Body: {request.Body}");
    }
    public bool RequestHasBody(Request request)
    {
        return request.Method == "POST" || request.Method == "PUT";
    }
}
