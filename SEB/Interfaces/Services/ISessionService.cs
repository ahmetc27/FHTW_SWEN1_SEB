using SEB.Models;

namespace SEB.Interfaces;

public interface ISessionService
{
    public void CreateToken(User user);
}