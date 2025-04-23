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
        if(string.IsNullOrWhiteSpace(user.Username)) throw new BadRequestException("Username is null or empty");

        if(string.IsNullOrEmpty(user.Password)) throw new BadRequestException("Password is null or empty");

        if(userRepository.ExistUsername(user.Username)) throw new BadRequestException("Username is already taken");

        User? dbUser = userRepository.AddUser(user.Username, user.Password); // add user to database -> forward user repo
        return dbUser;
    }

    public User? ValidateUser(User user) // for token post /sessions
    {
        if(string.IsNullOrWhiteSpace(user.Username)) throw new BadRequestException("Username is null or empty");

        if(string.IsNullOrEmpty(user.Password)) throw new BadRequestException("Password is null or empty");

        User? dbUser = userRepository.GetUser(user.Username, user.Password);

        if(dbUser == null) throw new UnauthorizedException("User does not exist or wrong credentials");

        return dbUser;
    }
}