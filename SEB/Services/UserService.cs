using SEB.Interfaces;
using SEB.Models;

namespace SEB.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    public void RegisterUser(User user)
    {
        if(string.IsNullOrEmpty(user.Username) || string.IsNullOrEmpty(user.Password))
            throw new ArgumentException("Username or password is empty");

        // check if username is taken already -> forward user repo
        if(!_userRepository.Exists(user.Username))
        {
            _userRepository.AddUser(user.Username, user.Password); // add user to database -> forward user repo
        }
        else
        {
            throw new ArgumentException("Username is already taken");
        }
    }
}