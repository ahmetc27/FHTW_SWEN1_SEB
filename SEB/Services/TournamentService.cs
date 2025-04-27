using SEB.Exceptions;
using SEB.Interfaces;
using SEB.Models;
using SEB.Utils;

namespace SEB.Services;

public class TournamentService : ITournamentService
{
    private readonly IUserRepository userRepository;
    private readonly ISessionRepository sessionRepository;
    private readonly IHistoryRepository historyRepository;
    private readonly ITournamentRepository tournamentRepository;
    public TournamentService(IUserRepository userRepository, ISessionRepository sessionRepository, IHistoryRepository historyRepository, ITournamentRepository tournamentRepository)
    {
        this.userRepository = userRepository;
        this.sessionRepository = sessionRepository;
        this.historyRepository = historyRepository;
        this.tournamentRepository = tournamentRepository;
    }
    public TournamentResponse GetCurrentTournament(string token)
    {
        if(!sessionRepository.ExistToken(token))
            throw new UnauthorizedException(ErrorMessages.TokenNotFound);
        
        var tournament = tournamentRepository.GetCurrentTournament();

        if(tournament != null && tournament.Status == "active" && tournament.StartTime.AddMinutes(2) <= DateTime.UtcNow)
        {
            EvaluateTournament(tournament.Id);
            tournament = tournamentRepository.GetCurrentTournament();
        }

        if(tournament == null)
        {
            List<Tournament>? tournaments = tournamentRepository.GetAllTournaments();
            return new TournamentResponse
            {
                ActiveTournament = null,
                PastTournaments = tournaments
            };
        }

        return new TournamentResponse
        {
            ActiveTournament = tournament,
            PastTournaments = null
        };
    }

    public void EvaluateTournament(int tournamentId)
    {
        var participants = tournamentRepository.GetParticipants(tournamentId);
        
        if(participants == null || participants.Count == 0)
            throw new Exception("No participants in tournament");
        
        int maxCount = participants.Max(p => p.TotalCount);
        var winners = participants.Where(p => p.TotalCount == maxCount).ToList();

        foreach(var participant in participants)
        {
            int userId = participant.UserId;
            User? user = userRepository.GetUserById(userId);
            
            if(user == null)
            {
                Logger.Error($"User {userId} not found when evaluating tournament");
                continue;
            }

            if(winners.Contains(participant))
            {
                if(winners.Count > 1)
                {
                    user.Elo += 1;
                    Logger.Info($"User {userId} tied for first place in tournament {tournamentId}, +1 ELO");
                }
                else
                {
                    user.Elo += 2;
                    Logger.Info($"User {userId} won tournament {tournamentId}, +2 ELO");
                }
            }
            else
            {
                user.Elo -= 1;
                Logger.Info($"User {userId} lost in tournament {tournamentId}, -1 ELO");
            }
            userRepository.UpdateElo(userId, user.Elo);
        }
        tournamentRepository.EndTournament(tournamentId);
        Logger.Success($"Tournament {tournamentId} evaluated and closed successfully");
    }
}