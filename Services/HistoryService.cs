using System.Text.Json;
using SEB.Models;
using SEB.Repositories;
using SEB.Http;

namespace SEB.Services
{
    public class HistoryService
    {
        private Response response = new();
        private UserRepository userRepository = new();
        private HistoryRepository historyRepository = new();
        private SessionRepository sessionRepository = new();
        private TournamentRepository tournamentRepository = new();
        private AuthService authService;

        public HistoryService()
        {
            authService = new AuthService(sessionRepository);
        }
    
        public void GetHistory(StreamWriter writer, Request request)
        {
            if(!authService.IsAuthorized(request, response, out string username))
            {
                response.SendUnauthorized(writer, "Unauthorized");
                return;
            }

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

        public void AddHistoryEntry(StreamWriter writer, Request request)
        {
            if(!authService.IsAuthorized(request, response, out string username))
            {
                response.SendUnauthorized(writer, "Unauthorized");
                return;
            }
            
            User? user = userRepository.GetUser(username);
            
            if(user == null)
            {
                response.SendNotFound(writer, "User not found");
                return;
            }
            
            // Parse the request body
            try
            {
                var historyRequest = JsonSerializer.Deserialize<HistoryRequest>(request.Body);
                if(historyRequest == null)
                {
                    response.SendBadRequest(writer, "Invalid request format");
                    return;
                }
                
                // Check if there's an active tournament or create a new one
                int? tournamentId = tournamentRepository.GetActiveTournamentId();
                if (!tournamentId.HasValue)
                {
                    // Create a new tournament
                    tournamentId = tournamentRepository.CreateTournament();
                    Console.WriteLine($"Created new tournament with ID: {tournamentId}");
                }
                
                // Add the history entry
                int historyId = historyRepository.AddHistoryEntry(
                    user.Id, 
                    historyRequest.Count, 
                    historyRequest.DurationInSeconds,
                    tournamentId);
                    
                response.SendCreated(writer, JsonSerializer.Serialize(new { 
                    Message = "History entry added successfully",
                    HistoryId = historyId,
                    TournamentId = tournamentId
                }));
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Error adding history entry: {ex.Message}");
                response.SendInternalError(writer, "Error processing history entry");
            }
        }
    }
    // Class to deserialize the request body
    class HistoryRequest
    {
        public string Name { get; set; } = "PushUps";
        public int Count { get; set; }
        public int DurationInSeconds { get; set; }
    }
}