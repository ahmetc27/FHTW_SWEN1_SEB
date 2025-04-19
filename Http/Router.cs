using SEB.Services;

namespace SEB.Http
{
    public class Router
    {
        private readonly UserService userService = new();
        private readonly SessionService sessionService = new();
        private readonly StatsService statsService = new();
        private readonly ScoreboardService scoreboardService = new();
        private readonly HistoryService historyService = new();
        private readonly TournamentService tournamentService = new();

        public void Route(StreamWriter writer, Request request)
        {
            if(request.Method == "POST" && request.Path == "/users")
            {
                userService.PostUser(writer, request);
            }
            else if(request.Method == "POST" && request.Path == "/sessions")
            {
                sessionService.PostSessions(writer, request);
            }
            else if(request.Method == "GET" && request.Path == "/users")
            {
                userService.GetAllUser(writer);
            }
            else if(request.Method == "GET" && request.Path.Contains("/users"))
            {
                userService.GetUser(writer, request);
            }
            else if(request.Method == "PUT" && request.Path.Contains("/users"))
            {
                userService.UpdateUser(writer, request);
            }
            else if(request.Method == "GET" && request.Path == "/stats")
            {
                statsService.GetStats(writer, request);
            }
            else if(request.Method == "GET" && request.Path == "/score")
            {
                scoreboardService.GetScoreboard(writer, request);
            }
            else if(request.Method == "GET" && request.Path == "/history")
            {
                historyService.GetHistory(writer, request);
            }
            else if(request.Method == "GET" && request.Path == "/tournament")
            {
                tournamentService.GetCurrentTournament(writer, request);
            }
            else if(request.Method == "POST" && request.Path == "/history")
            {
                historyService.AddHistoryEntry(writer, request);
            }
        }
    }
}