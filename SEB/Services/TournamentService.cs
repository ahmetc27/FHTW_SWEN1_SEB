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
    public Tournament? GetCurrentTournament(string token)
    {
        if(!sessionRepository.ExistToken(token))
            throw new UnauthorizedException(ErrorMessages.TokenNotFound);
        
        return tournamentRepository.GetCurrentTournament();
    }
}