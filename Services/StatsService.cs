using SEB.Http;
using SEB.Repositories;
using System.Text.Json;

namespace SEB.Services
{
    public class StatsService
    {
        private SessionRepository sessionRepository = new SessionRepository();
        private StatsRepository statsRepository = new StatsRepository();
        public Response response = new Response();
        public void GetStats(StreamWriter writer, Request request)
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
            string username = sessionRepository.GetUsernameByToken(receivedToken);

            int elo;
            int totalPushups;

            (elo, totalPushups) = statsRepository.GetStats(username);

            var stats = new { 
                Elo = elo, 
                TotalPushups = totalPushups 
            };

            string json = JsonSerializer.Serialize(stats);

            response.SendOk(writer, json);
        }
    }
}