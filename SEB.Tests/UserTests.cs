using NUnit.Framework;
using Moq;
using SEB.Interfaces;
using SEB.Services;
using SEB.DTOs;
using SEB.Models;
using SEB.Utils;
using SEB.Exceptions;

namespace SEB.Tests;

public class UserTests
{
    [Test]
    public void Register_CheckDuplicateUsername_ReturnsCreated()
    {
        // Arrange
        var userCreds = new UserCredentials
        {
            Username = "newUser",
            Password = "123"
        };

        var expectedUser = new User
        {
            UserId = 1,
            Username = "newUser",
            Password = "123",
            Elo = 100
        };

        Mock<IUserRepository> mockUserRepo = new Mock<IUserRepository>();
        
        mockUserRepo.Setup(repo => repo.ExistUsername(userCreds.Username))
            .Returns(false);
        
        mockUserRepo.Setup(repo => repo.AddUser("newUser", "123"))
            .Returns(expectedUser);

        var userService = new UserService(mockUserRepo.Object);

        // Act
        var result = userService.RegisterUser(userCreds);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Username, Is.EqualTo("newUser"));
        Assert.That(result.UserId, Is.EqualTo(1));
        Assert.That(result.Elo, Is.EqualTo(100));

        mockUserRepo.Verify(repo => repo.ExistUsername("newUser"), Times.Once);
        mockUserRepo.Verify(repo => repo.AddUser(It.Is<string>(u => u == "newUser"), It.Is<string>(p => p == "123")), Times.Once);
    }

    [Test]
    public void Register_UsernameAlreadyExists_ThrowsConflictException()
    {
        var creds = new UserCredentials
        {
            Username = "existing",
            Password = "123"
        };

        var mockRepo = new Mock<IUserRepository>();

        mockRepo.Setup(repo => repo.ExistUsername("existing"))
            .Returns(true);

        var service = new UserService(mockRepo.Object);

        Assert.Throws<ConflictException>(() => service.RegisterUser(creds));    
    }

    [Test]
    public void UpdateUserProfile_WithInvalidToken_ThrowsUnauthorizedException()
    {
        // Arrange
        var user = new User()
        {
            UserId = 1,
            Username = "newUser",
            Password = "123",
            Elo = 100,
            Token = "newUser-sebToken",
            Name = "",
            Bio = "",
            Image = ""
        };

        var userProfile = new UserProfile()
        {
            Name = "test",
            Bio = "I like coding",
            Image = ":D"
        };

        Mock<IUserRepository> mockUserRepo = new Mock<IUserRepository>();

        mockUserRepo.Setup(repo => repo.GetUserByUsernameAndToken(user.Username, user.Token))
            .Returns((User)null);
        
        var userService = new UserService(mockUserRepo.Object);

        // Act & Assert
        Assert.Throws<UnauthorizedException>(() => userService.UpdateUserProfile(user.Username, user.Token, userProfile));
        mockUserRepo.Verify(repo => repo.GetUserByUsernameAndToken(It.Is<string>(u => u == "newUser"), It.Is<string>(p => p == "newUser-sebToken")), Times.Once);
    }
}