using System.Text;
using SEB.Services;

namespace SEB.Http
{
    public class ServerService
    {
        private UserService userService = new UserService();
        private SessionService sessionsService = new SessionService();
        private StatsService statsService = new StatsService();
        private ScoreboardService scoreboardService = new ScoreboardService();
        private HistoryService historyService = new HistoryService();
        private TournamentService tournamentService = new TournamentService();

        public void ParseRequest(StreamReader reader, Request request)
        {
            ParseRequestLine(reader, request);
            ParseHeaders(reader, request);
            if(MethodHasBody(request)) ParseBody(reader, request);
        }
        public void ParseRequestLine(StreamReader reader, Request request)
        {
            string line = reader.ReadLine() ?? ""; // POST /users HTTP/1.1

            string[] arr = line.Split(' ');
            request.Method = arr[0];
            request.Path = arr[1];
            request.Version = arr[2];

            Console.WriteLine($"Method: {request.Method}, Path: {request.Path}");            
        }
        public void ParseHeaders(StreamReader reader, Request request)
        {
            while(true)
            { 
                string line = reader.ReadLine() ?? ""; // Content-Type: application/json
                if(line == "") break;

                string[] arr = line.Split(':', 2);
                string headerName = arr[0];
                string headerValue = arr[1].Trim();
                request.Headers[headerName] = headerValue;
                //Console.WriteLine($"Header: {headerName} = {headerValue}");

                if(headerName == "Content-Length")
                {
                    request.ContentLength = int.Parse(headerValue);
                }
            }
        }
        public void ParseBody(StreamReader reader, Request request)
        {
            StringBuilder requestBody = new StringBuilder();
            if(request.ContentLength > 0)
            {
                char[] chars = new char[1024];
                int bytesReadTotal = 0;
                while(bytesReadTotal < request.ContentLength)
                {
                    var bytesRead = reader.Read(chars, 0, chars.Length);
                    bytesReadTotal += bytesRead;
                    if(bytesRead == 0)
                    {
                        break;  // no more data available
                    }
                    requestBody.Append(chars, 0, bytesRead);
                }
            }
            request.Body = requestBody.ToString();
            //Console.WriteLine($"Body: {request.Body}");
        }
        public bool MethodHasBody(Request request)
        {
            return request.Method == "POST" || request.Method == "PUT";
        }
    }
}