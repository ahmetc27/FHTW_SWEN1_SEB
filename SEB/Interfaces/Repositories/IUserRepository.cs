using SEB.Models;

namespace SEB.Interfaces;

public interface IUserRepository
{
    public bool ExistUsername(string username);
    public User? AddUser(string username, string password);
    public User? GetUser(string username, string password);
    public User? GetUserByUsernameAndToken(string username, string token);
    public void UpdateUserProfile(User user);
    public int? GetIdByToken(string token);
}