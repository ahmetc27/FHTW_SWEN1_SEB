using NUnit.Framework;
using Moq;
using SEB.Models;
using SEB.Http;
using SEB.Controller;
using SEB.Interfaces;
using SEB.Exceptions;
using System.Text.Json;

namespace SEB.Tests;

public class UserProfileTests
{
    [Test]
    public void GetUserByName_ValidToken_ReturnsUserProfile()
    {
        // Arrange
        var username = "testuser";
        var token = "testuser-sebToken";
        
        var request = new Request
        {
            Method = "GET",
            Path = $"/users/{username}",
            Headers = new Dictionary<string, string>
            {
                { "Authorization", $"Basic {token}" }
            }
        };
        
        var expectedUser = new User
        {
            Id = 1,
            Username = username,
            Password = "password123",
            Elo = 100,
            Token = token,
            Bio = "Test bio",
            Image = ":)"
        };
        
        var mockUserService = new Mock<IUserService>();
        mockUserService
            .Setup(us => us.ValidateUserAccess(username, token))
            .Returns(expectedUser);
        
        using var memoryStream = new MemoryStream();
        using var writer = new StreamWriter(memoryStream) { AutoFlush = true };
        
        // Act
        UserController.GetUserByName(writer, request, mockUserService.Object);
        
        // Assert
        memoryStream.Seek(0, SeekOrigin.Begin);
        var reader = new StreamReader(memoryStream);
        string response = reader.ReadToEnd();
        
        Assert.That(response.Contains("200 OK"), Is.True);
        Assert.That(response.Contains(username), Is.True);
        Assert.That(response.Contains("Test bio"), Is.True);
        
        mockUserService.Verify(us => us.ValidateUserAccess(username, token), Times.Once);
    }
    [Test]
    public void UpdateUserProfile_ValidRequest_UpdatesAndReturnsProfile()
    {
        // Arrange
        var username = "testuser";
        var token = "testuser-sebToken";
        
        var request = new Request
        {
            Method = "PUT",
            Path = $"/users/{username}",
            Headers = new Dictionary<string, string>
            {
                { "Authorization", $"Basic {token}" }
            },
            Body = "{\"Name\":\"TestUpdate\", \"Bio\":\"Updated bio\", \"Image\":\":D\"}"
        };
        
        var dbUser = new User
        {
            Id = 1,
            Username = username,
            Password = "password123",
            Elo = 100,
            Token = token,
            Bio = "Original bio",
            Image = ":)"
        };
        
        var mockUserService = new Mock<IUserService>();
        mockUserService
            .Setup(us => us.ValidateUserAccess(username, token))
            .Returns(dbUser);
        
        mockUserService
            .Setup(us => us.CheckUserProfile(It.IsAny<UserProfile>(), dbUser))
            .Callback<UserProfile, User>((profile, user) => {
                user.Username = "TestUpdate";
                user.Bio = "Updated bio";
                user.Image = ":D";
            });
        
        using var memoryStream = new MemoryStream();
        using var writer = new StreamWriter(memoryStream) { AutoFlush = true };
        
        // Act
        UserController.UpdateUserProfile(writer, request, mockUserService.Object);
        
        // Assert
        memoryStream.Seek(0, SeekOrigin.Begin);
        var reader = new StreamReader(memoryStream);
        string response = reader.ReadToEnd();
        
        Assert.That(response.Contains("200 OK"), Is.True);
        Assert.That(response.Contains("Updated bio"), Is.True);
        Assert.That(response.Contains("TestUpdate"), Is.True);
        Assert.That(response.Contains(":D"), Is.True);
        
        mockUserService.Verify(us => us.ValidateUserAccess(username, token), Times.Once);
        mockUserService.Verify(us => us.CheckUserProfile(
            It.Is<UserProfile>(p => 
                p.Name == "TestUpdate" && 
                p.Bio == "Updated bio" && 
                p.Image == ":D"),
            It.IsAny<User>()), 
            Times.Once);
    }
}