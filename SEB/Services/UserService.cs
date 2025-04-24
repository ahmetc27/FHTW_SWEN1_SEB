using SEB.DTOs;
using SEB.Exceptions;
using SEB.Interfaces;
using SEB.Models;
using SEB.Utils;

namespace SEB.Services;

public class UserService : IUserService
{
    private readonly IUserRepository userRepository;
    public UserService(IUserRepository userRepository)
    {
        this.userRepository = userRepository;
    }
    public User RegisterUser(UserCredentials credentials)
    {
        RequestHelper.ValidateCredentials(credentials.Username, "Username");
        RequestHelper.ValidateCredentials(credentials.Password, "Password");

        if(userRepository.ExistUsername(credentials.Username))
            throw new ConflictException(ErrorMessages.UsernameTaken);

        return userRepository.AddUser(credentials.Username, credentials.Password);
    }

    public User AuthenticateUser(UserCredentials credentials) // for token post /sessions
    {
        RequestHelper.ValidateCredentials(credentials.Username, "Username");
        RequestHelper.ValidateCredentials(credentials.Password, "Password");

        return userRepository.GetUser(credentials.Username, credentials.Password)
            ?? throw new UnauthorizedException(ErrorMessages.InvalidCredentials);
    }

    public User ValidateUserAccess(string username, string token)
    {
        RequestHelper.ValidateCredentials(username, "Username");
        RequestHelper.ValidateCredentials(token, "Token");

        return userRepository.GetUserByUsernameAndToken(username, token)
            ?? throw new UnauthorizedException(ErrorMessages.InvalidUsernameOrToken);
    }

    public User UpdateUserProfile(string username, string token, UserProfile requestUserProfile)
    {
        // 1. Validate user access (throws if invalid)
        User dbUser = ValidateUserAccess(username, token);

        // 2. Apply updates (with validation if needed)
        if(!string.IsNullOrWhiteSpace(requestUserProfile.Name))
            dbUser.Name = requestUserProfile.Name;
        
        if(!string.IsNullOrWhiteSpace(requestUserProfile.Bio))
            dbUser.Bio = requestUserProfile.Bio;
        
        if(!string.IsNullOrWhiteSpace(requestUserProfile.Image))
            dbUser.Image = requestUserProfile.Image;
        
        // 3. Persist changes
        userRepository.Update(dbUser);

        return dbUser;
    }
}