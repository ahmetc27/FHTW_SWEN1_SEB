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
    public void RegisterUser(User user)
    {
        if(string.IsNullOrEmpty(user.Username) || string.IsNullOrEmpty(user.Password))
            throw new ArgumentException("Username or password is empty");

        // check if username is taken already -> forward user repo
        if(!userRepository.ExistUsername(user.Username))
            userRepository.AddUser(user.Username, user.Password); // add user to database -> forward user repo

        else
            throw new ArgumentException("Username is already taken");
    }

    public User? ValidateUser(User user)
    {
        if(user == null) 
            throw new ArgumentNullException("User not found");

        if(string.IsNullOrEmpty(user.Username) || string.IsNullOrEmpty(user.Password))
            throw new ArgumentException("Username or password is empty");

        User? dbUser = userRepository.GetUser(user.Username, user.Password);

        if(dbUser == null)
            throw new ArgumentException("Wrong credentials");

        return dbUser;
    }
}