using System.Text.Json;
using SEB.Repositories;
using SEB.Server;

namespace SEB.Services
{
    public class ScoreboardService
    {
        private Response response = new Response();
        private SessionRepository sessionRepository = new SessionRepository("Host=localhost;Username=postgres;Password=postgres;Database=postgres");
        private ScoreboardRepository scoreboardRepository = new ScoreboardRepository("Host=localhost;Username=postgres;Password=postgres;Database=postgres");
    
        public void GetScoreboard(StreamWriter writer, Request request)
        {
            if(!request.Headers.ContainsKey("Authorization"))
            {
                response.SendUnauthorized(writer, "Authorization header required");
                return;
            }

            string authHeader = request.Headers["Authorization"];
            string receivedToken = authHeader.Replace("Basic ", "").Trim();

            if(!sessionRepository.ExistToken(receivedToken))
            {
                response.SendUnauthorized(writer, "Invalid token");
                return;
            }

            var scoreboard = scoreboardRepository.GetScoreboard();
            string json = JsonSerializer.Serialize(scoreboard);
            
            response.SendOk(writer, json);
            
        }
    }
}