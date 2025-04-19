using System.Text.Json;
using SEB.Models;
using SEB.Repositories;
using SEB.Server;

namespace SEB.Services
{
    public class HistoryService
    {
        private Response response = new Response();
        private UserRepository userRepository = new UserRepository("Host=localhost;Username=postgres;Password=postgres;Database=postgres");
        private HistoryRepository historyRepository = new HistoryRepository("Host=localhost;Username=postgres;Password=postgres;Database=postgres");
        private SessionRepository sessionRepository = new SessionRepository("Host=localhost;Username=postgres;Password=postgres;Database=postgres");
    
        public void GetHistory(StreamWriter writer, Request request)
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
            User? user = userRepository.GetUser(username);

            if(user == null)
            {
                response.SendNotFound(writer, "User not found");
                return;
            }
            
            var history = historyRepository.GetUserHistory(user.Id);
            string json = JsonSerializer.Serialize(history);

            response.SendOk(writer, json);
        }
    }
}