using System.Text.Json;
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

        User user = userRepository.AddUser(credentials.Username, credentials.Password);

        return new User
        {
            UserId = user.UserId,
            Username = user.Username,
            Password = user.Password,
            Elo = user.Elo,
            Name = user.Name,
            Bio = user.Bio,
            Image = user.Image
        };
    }

    public User? ValidateUser(User user) // for token post /sessions
    {
        if(string.IsNullOrWhiteSpace(user.Username)) throw new BadRequestException("Username must not be empty");

        if(string.IsNullOrEmpty(user.Password)) throw new BadRequestException("Password must not be empty");

        User? dbUser = userRepository.GetUser(user.Username, user.Password);

        if(dbUser == null) throw new UnauthorizedException("User does not exist or wrong credentials");

        return dbUser;
    }

    public User? ValidateUserAccess(string username, string token)
    {
        if(string.IsNullOrWhiteSpace(username)) throw new BadRequestException("Username must not be empty");

        if(string.IsNullOrWhiteSpace(token)) throw new BadRequestException("Authorization token is missing or empty");

        User? dbUser = userRepository.GetUserByUsernameAndToken(username, token);

        if(dbUser == null) throw new UnauthorizedException("Invalid username or token");

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
            throw new BadRequestException($"{fieldName} must not be empty");
    }
}