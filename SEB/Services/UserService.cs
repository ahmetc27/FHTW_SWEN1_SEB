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
        ValidateCredentials(credentials.Username, "Username");
        ValidateCredentials(credentials.Password, "Password");

        if(userRepository.ExistUsername(credentials.Username))
        {
            Logger.Error($"Username already taken: {credentials.Username}");
            throw new BadRequestException("Username is already taken");
        }

        return userRepository.AddUser(credentials.Username, credentials.Password);
    }

    public User AuthenticateUser(UserCredentials credentials) // for token post /sessions
    {
        ValidateCredentials(credentials.Username, "Username");
        ValidateCredentials(credentials.Password, "Password");

        User? user = userRepository.GetUser(credentials.Username, credentials.Password);

        if(user == null)
        {
            Logger.Error("User does not exist or wrong credentials");
            throw new UnauthorizedException("User does not exist or wrong credentials");
        }

        return user;
    }

    public User ValidateUserAccess(string username, string token)
    {
        ValidateCredentials(username, "Username");
        ValidateCredentials(token, "Token");

        User? dbUser = userRepository.GetUserByUsernameAndToken(username, token);

        if(dbUser == null)
        {
            Logger.Error("Access denied: invalid username or token");
            throw new UnauthorizedException("Access denied: invalid username or token");
        }

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

    private void ValidateCredentials(string value, string fieldName)
    {
        if(string.IsNullOrWhiteSpace(value))
        {
            Logger.Error($"{fieldName} invalid");
            throw new BadRequestException($"{fieldName} invalid");
        }
    }
}