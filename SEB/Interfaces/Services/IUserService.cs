using SEB.Models;
using SEB.DTOs;

namespace SEB.Interfaces;

public interface IUserService
{
    public User RegisterUser(UserCredentials credentials);
    public User AuthenticateUser(UserCredentials credentials);
    public User ValidateUserAccess(string username, string token);
    public void CheckUserProfile(UserProfile requestUserProfile, User dbUser);
}