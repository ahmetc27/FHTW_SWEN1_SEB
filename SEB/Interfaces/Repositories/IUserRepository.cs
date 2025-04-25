using SEB.Models;

namespace SEB.Interfaces;

public interface IUserRepository
{
    bool ExistUsername(string username);
    User AddUser(string username, string password);
    User? GetUser(string username, string password);
    User? GetUserByUsernameAndToken(string username, string token);
    void Update(User user);
    int? GetIdByToken(string token);
    User? GetUserById(int userId);
    void UpdateElo(int userId, int newElo);
}