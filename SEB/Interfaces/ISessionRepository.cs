using SEB.Interfaces;

public interface ISessionRepository
{
    public bool ExistToken(string token);
    public void SaveToken(string username, string password, string token);
}