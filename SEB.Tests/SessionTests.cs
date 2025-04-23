using NUnit.Framework;
using Moq;
using SEB.Models;
using SEB.Http;
using SEB.Controller;
using SEB.Interfaces;
using System.Text.Json;

namespace SEB.Tests;

public class SessionTests
{
    [Test]
    public void Login_ValidCredentials_ReturnsTokenAndOkStatus()
    {
        // Arrange
        var loginRequest = new Request
        {
            Body = "{\"Username\":\"testuser\", \"Password\":\"password123\"}"
        };
        
        // Set up a mock user that will be returned when ValidateUser is called
        var dbUser = new User 
        { 
            Username = "testuser", 
            Password = "password123",
            Id = 1,
            Elo = 100
        };
        
        var mockUserService = new Mock<IUserService>();
        mockUserService
            .Setup(us => us.ValidateUser(It.Is<User>(u => 
                u.Username == "testuser" && u.Password == "password123")))
            .Returns(dbUser);
        
        var mockSessionService = new Mock<ISessionService>();
        mockSessionService
            .Setup(ss => ss.CreateToken(It.IsAny<User>()))
            .Callback<User>(user => user.Token = "testuser-sebToken");
        
        using var memoryStream = new MemoryStream();
        using var writer = new StreamWriter(memoryStream) { AutoFlush = true };
        
        // Act
        SessionController.Login(writer, loginRequest, mockUserService.Object, mockSessionService.Object);
        
        // Assert
        memoryStream.Seek(0, SeekOrigin.Begin);
        var reader = new StreamReader(memoryStream);
        string response = reader.ReadToEnd();
        
        Assert.That(response.Contains("201 Created"), Is.True);
        Assert.That(response.Contains("testuser-sebToken"), Is.True);
        
        mockUserService.Verify(us => us.ValidateUser(It.Is<User>(u => 
            u.Username == "testuser" && u.Password == "password123")), Times.Once);
        mockSessionService.Verify(ss => ss.CreateToken(It.IsAny<User>()), Times.Once);
    }
}