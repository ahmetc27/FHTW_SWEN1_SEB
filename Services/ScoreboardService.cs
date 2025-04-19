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
        private AuthService authService;
        public ScoreboardService()
        {
            authService = new AuthService(sessionRepository);
        }
    
        public void GetScoreboard(StreamWriter writer, Request request)
        {
            if(!authService.IsAuthorized(request, response, out string username))
            {
                response.SendUnauthorized(writer, "Unauthorized");
                return;
            }

            var scoreboard = scoreboardRepository.GetScoreboard();
            string json = JsonSerializer.Serialize(scoreboard);
            
            response.SendOk(writer, json);
            
        }
    }
}