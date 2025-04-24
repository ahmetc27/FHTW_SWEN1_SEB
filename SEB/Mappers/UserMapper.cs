namespace SEB.Mappers;

using SEB.Models;
using SEB.DTOs;

public static class UserMapper
{
    public static UserResponse ToUserResponse(User user)
    {
        return new UserResponse
        {
            Id = user.UserId,
            Username = user.Username,
            Elo = user.Elo,
            Name = user.Name,
            Bio = user.Bio,
            Image = user.Image
        };
    }
}
