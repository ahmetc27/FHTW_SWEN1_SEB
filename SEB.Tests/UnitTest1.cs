using NUnit.Framework;
using Moq;
using SEB.Models;
using SEB.Http;
using SEB.Controller;
using SEB.Interfaces;

namespace SEB.Tests;

public class Tests
{
    [SetUp]
    public void Setup()
    {

    }

    [Test]
    public void Register_CreatesUserAndReturns201()
    {
        // Arrange
        var user = new User { Username = "testuser", Password = "123" };
        string jsonBody = "{\"Username\":\"testuser\", \"Password\":\"123\"}";

        var request = new Request
        {
            Body = jsonBody
        };
        
        var mockUserService = new Mock<IUserService>();
        mockUserService.Setup(us => us.RegisterUser(It.IsAny<User>()));

        using var memoryStream = new MemoryStream();
        using var writer = new StreamWriter(memoryStream) { AutoFlush = true };

        // Act
        UserController.Register(writer, request, mockUserService.Object);

        // Assert
        memoryStream.Seek(0, SeekOrigin.Begin);
        var reader = new StreamReader(memoryStream);
        string response = reader.ReadToEnd();

        Assert.That(response.Contains("201 Created"), Is.True);
        Assert.That(response.Contains("testuser"), Is.True);
        mockUserService.Verify(us => us.RegisterUser(It.Is<User>(u => u.Username == "testuser")), Times.Once);
    }
}
