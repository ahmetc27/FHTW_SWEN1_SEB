using SEB.Interfaces;
using SEB.Models;

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

        // check if this token is already in db
        if(!sessionRepository.ExistToken(user.Token))
        {
            sessionRepository.SaveToken(user.Username, user.Password, user.Token);
        }        
        else
            throw new Exception("Token already exists");
    }
}