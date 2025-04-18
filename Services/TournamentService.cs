using System.Text.Json;
using SEB.Repositories;
using SEB.Http;

namespace SEB.Services
{
    public class TournamentService
    {
        private TournamentRepository tournamentRepository = new();
        private SessionRepository sessionRepository = new();
        private Response response = new();
        public void GetCurrentTournament(StreamWriter writer, Request request)
        {
            if(!request.Headers.ContainsKey("Authorization"))
            {
                response.SendUnauthorized(writer, "Authorization header required");
                return;
            }

            string authHeader = request.Headers["Authorization"];
            string receivedToken = authHeader.Replace("Basic ", "");

            if(!sessionRepository.ExistToken(receivedToken))
            {
                response.SendUnauthorized(writer, "Invalid token");
                return;
            }

            var tournament = tournamentRepository.GetCurrentTournament();
            string json = JsonSerializer.Serialize(tournament);

            if(tournament == null)
            {
                response.SendNotFound(writer, "No active tournament found");
                return;
            }

            response.SendOk(writer, json);
        }
    }
}