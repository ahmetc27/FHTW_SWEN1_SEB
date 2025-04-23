using SEB.Models;

namespace SEB.Interfaces;

public interface IUserRepository
{
    public bool ExistUsername(string username);
    public void AddUser(string username, string password);
    public User? GetUser(string username, string password);
}