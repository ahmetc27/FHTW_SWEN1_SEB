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

    public HistoryService(IUserRepository userRepository, ISessionRepository sessionRepository, IHistoryRepository historyRepository)
    {
        this.userRepository = userRepository;
        this.sessionRepository = sessionRepository;
        this.historyRepository = historyRepository;
    }
    public History GetUserHistoryData(string token)
    {
        if(string.IsNullOrWhiteSpace(token))
            throw new BadRequestException("Token is missing or empty");

        if(!sessionRepository.ExistToken(token))
            throw new UnauthorizedException("Token does not exist");
        
        int? userId = userRepository.GetIdByToken(token)
            ?? throw new BadRequestException("User Id invalid");

        History history = historyRepository.GetHistoryByUserId(userId.Value)
            ?? throw new BadRequestException("History invalid");            
        
        return history;
    }
}