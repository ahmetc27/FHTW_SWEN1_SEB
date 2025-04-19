using System.Text.Json;
using SEB.Repositories;
using SEB.Http;

namespace SEB.Services
{
    public class ScoreboardService
    {
        private Response response = new();
        private SessionRepository sessionRepository = new();
        private ScoreboardRepository scoreboardRepository = new();
    
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