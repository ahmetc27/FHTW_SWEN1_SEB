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
            throw new BadRequestException("Username is already taken");

        return userRepository.AddUser(credentials.Username, credentials.Password);
    }

    public User AuthenticateUser(UserCredentials credentials) // for token post /sessions
    {
        RequestHelper.ValidateCredentials(credentials.Username, "Username");
        RequestHelper.ValidateCredentials(credentials.Password, "Password");

        User? user = userRepository.GetUser(credentials.Username, credentials.Password)
            ?? throw new UnauthorizedException("User does not exist or wrong credentials");

        return user;
    }

    public User ValidateUserAccess(string username, string token)
    {
        RequestHelper.ValidateCredentials(username, "Username");
        RequestHelper.ValidateCredentials(token, "Token");

        User? dbUser = userRepository.GetUserByUsernameAndToken(username, token)
            ?? throw new UnauthorizedException("Access denied: invalid username or token");

        return dbUser;
    }

    public void CheckUserProfile(UserProfile requestUserProfile, User dbUser)
    {
        if(!string.IsNullOrWhiteSpace(requestUserProfile.Name))
            dbUser.Name = requestUserProfile.Name;
        
        if(!string.IsNullOrWhiteSpace(requestUserProfile.Bio))
            dbUser.Bio = requestUserProfile.Bio;
        
        if(!string.IsNullOrWhiteSpace(requestUserProfile.Image))
            dbUser.Image = requestUserProfile.Image;
            
        userRepository.UpdateUserProfile(dbUser);
    }
}