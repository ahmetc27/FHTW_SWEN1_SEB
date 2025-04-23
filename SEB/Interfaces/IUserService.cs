using SEB.Models;

namespace SEB.Interfaces;

public interface IUserService
{
    public User? RegisterUser(User user);
    public User? ValidateUser(User user);
}