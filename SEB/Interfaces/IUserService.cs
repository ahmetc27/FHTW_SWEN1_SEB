using SEB.Models;

namespace SEB.Interfaces;

public interface IUserService
{
    public void RegisterUser(User user);
    public User? ValidateUser(User user);
}