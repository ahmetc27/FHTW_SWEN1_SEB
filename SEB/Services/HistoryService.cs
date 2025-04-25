using SEB.DTOs;
using SEB.Exceptions;
using SEB.Interfaces;
using SEB.Models;
using SEB.Utils;

namespace SEB.Service;

public class HistoryService : IHistoryService
{
    private readonly IUserRepository userRepository;
    private readonly ISessionRepository sessionRepository;

    private readonly IHistoryRepository historyRepository;
    private readonly ITournamentRepository tournamentRepository;

    public HistoryService(IUserRepository userRepository, ISessionRepository sessionRepository, IHistoryRepository historyRepository, ITournamentRepository tournamentRepository) 
    {
        this.userRepository = userRepository;
        this.sessionRepository = sessionRepository;
        this.historyRepository = historyRepository;
        this.tournamentRepository = tournamentRepository;
    }
    public History GetUserHistoryData(string token)
    {
        if(!sessionRepository.ExistToken(token))
            throw new UnauthorizedException(ErrorMessages.TokenNotFound);
        
        int userId = userRepository.GetIdByToken(token)
            ?? throw new BadRequestException(ErrorMessages.UserIdNotFound);

        History? history = historyRepository.GetHistoryByUserId(userId);
        
        if(history == null)
        {
            Logger.Warn($"No history found for user {userId}, returning empty history.");
            history = new History
            {
                Count = 0,
                Duration = 0
            };        
        }
        
        return history;
    }

    public History LogPushups(string token, HistoryRequest historyRequest)
    {
        if(historyRequest.Count <= 0 || historyRequest.DurationInSeconds <= 0)
            throw new BadRequestException(ErrorMessages.PositiveNumbersRequired);

        int userId = userRepository.GetIdByToken(token)
            ?? throw new BadRequestException(ErrorMessages.UserIdNotFound);

        var history = new History
        {
            Name = historyRequest.Name,
            Count = historyRequest.Count,
            Duration = historyRequest.DurationInSeconds
        };

        var activeTournament = tournamentRepository.GetCurrentTournament();

        if(activeTournament == null)
            activeTournament = tournamentRepository.StartNewTournament();

        if(activeTournament.StartTime.AddMinutes(2) <= DateTime.Now)
        {
            tournamentRepository.EndTournament(activeTournament.Id);
            activeTournament = tournamentRepository.StartNewTournament();
        }
        
        var participant = tournamentRepository.GetParticipant(activeTournament.Id, userId);

        if(participant == null)
        {
            tournamentRepository.AddParticipant(activeTournament.Id, userId, history.Count, history.Duration);
        }
        else
        {
            participant.TotalCount += history.Count;
            participant.TotalDuration += history.Duration;
            tournamentRepository.UpdateParticipant(activeTournament.Id, userId, participant);
        }

        return historyRepository.Add(userId, history);
    }
}