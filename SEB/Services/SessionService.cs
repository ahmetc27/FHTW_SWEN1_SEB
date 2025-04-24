using SEB.Exceptions;
using SEB.Interfaces;
using SEB.Models;
using SEB.Utils;

namespace SEB.Services;

public class SessionService : ISessionService
{
    private readonly IUserRepository userRepository;
    private readonly ISessionRepository sessionRepository;
    public SessionService(IUserRepository userRepository, ISessionRepository sessionRepository)
    {
        this.userRepository = userRepository;
        this.sessionRepository = sessionRepository;
    }
    public void CreateToken(User user)
    {
        user.Token = $"{user.Username}-sebToken";

        if(sessionRepository.ExistToken(user.Token)) // check if this token is already in db
            throw new UnauthorizedException(ErrorMessages.TokenAlreadyExists);

        sessionRepository.SaveToken(user.Username, user.Password, user.Token);            
    }
}