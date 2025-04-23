namespace SEB.Interfaces;

public interface IUserRepository
{
    public bool Exists(string username);
    public void AddUser(string username, string password);
}