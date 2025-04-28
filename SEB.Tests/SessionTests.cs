using NUnit.Framework;
using Moq;
using SEB.Interfaces;
using SEB.Services;
using SEB.DTOs;
using SEB.Models;
using SEB.Utils;
using SEB.Exceptions;

namespace SEB.Tests;

public class SessionTests
{
    [Test]
    public void Login_WithValidCredentials_CreatesToken()
    {
        // Arrange
        var user = new User
        {
            Username = "newUser",
            Password = "123"
        };

        Mock<IUserRepository> mockUserRepo = new Mock<IUserRepository>();
        Mock<ISessionRepository> mockSessionRepo = new Mock<ISessionRepository>();

        mockSessionRepo.Setup(repo => repo.ExistToken("newUser-sebToken"))
            .Returns(false);
        
        mockSessionRepo.Setup(repo => repo.SaveToken(user.Username, user.Password, "newUser-sebToken"));
        
        var sessionService = new SessionService(mockUserRepo.Object, mockSessionRepo.Object);

        // Act
        sessionService.CreateToken(user);

        // Assert
        Assert.That(user.Token, Is.EqualTo("newUser-sebToken"));
        mockSessionRepo.Verify(repo => repo.ExistToken("newUser-sebToken"), Times.Once);
        mockSessionRepo.Verify(repo => repo.SaveToken("newUser", "123", "newUser-sebToken"), Times.Once);
    }

    [Test]
    public void Login_WithInvalidCredentials_ThrowsUnauthorizedException()
    {
        // Arrange
        var userCreds = new UserCredentials
        {
            Username = "newUser",
            Password = "123"
        };

        Mock<IUserRepository> mockUserRepo = new Mock<IUserRepository>();
        Mock<ISessionRepository> mockSessionRepo = new Mock<ISessionRepository>();

        mockUserRepo.Setup(repo => repo.GetUser(userCreds.Username, userCreds.Password))
            .Returns((User)null);
        
        var userService = new UserService(mockUserRepo.Object);

        // Act & Assert
        Assert.Throws<UnauthorizedException>(() => userService.AuthenticateUser(userCreds));
    }
}