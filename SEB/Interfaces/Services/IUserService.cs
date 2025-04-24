using SEB.Models;

namespace SEB.Interfaces;

public interface IUserService
{
    public User RegisterUser(UserCredentials credentials);
    public User? ValidateUser(User user);
    public User? ValidateUserAccess(string username, string token);
    public void CheckUserProfile(UserProfile requestUserProfile, User dbUser);
}