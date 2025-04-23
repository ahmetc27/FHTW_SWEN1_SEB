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
    public User? RegisterUser(User user)
    {
        if(string.IsNullOrWhiteSpace(user.Username)) throw new BadRequestException("Username must not be empty");

        if(string.IsNullOrEmpty(user.Password)) throw new BadRequestException("Password must not be empty");

        if(userRepository.ExistUsername(user.Username)) throw new BadRequestException("Username is already taken");

        User? dbUser = userRepository.AddUser(user.Username, user.Password);
        return dbUser;
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
        {
            if(userRepository.ExistUsername(requestUserProfile.Name))
                throw new BadRequestException("Username already taken");
            dbUser.Username = requestUserProfile.Name;
        }
        
        if(!string.IsNullOrWhiteSpace(requestUserProfile.Bio))
            dbUser.Bio = requestUserProfile.Bio;
        
        if(!string.IsNullOrWhiteSpace(requestUserProfile.Image))
            dbUser.Image = requestUserProfile.Image;
            
        userRepository.UpdateUserProfile(dbUser);
    }
}