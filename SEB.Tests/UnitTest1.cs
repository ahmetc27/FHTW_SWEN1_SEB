using NUnit.Framework;
using Moq;
using SEB.Models;
using SEB.Http;
using SEB.Controller;
using SEB.Interfaces;
using System.Text.Json;

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

    [Test]
    public void Register_DuplicateUser_ThrowsArgumentException()
    {
        // Arrange
        var request = new Request
        {
            Body = "{\"Username\":\"testuser\", \"Password\":\"123\"}"
        };

        var mockUserService = new Mock<IUserService>();
        mockUserService
            .Setup(us => us.RegisterUser(It.Is<User>(u => u.Username == "testuser")))
            .Throws(new ArgumentException("Username already taken"));

        using var memoryStream = new MemoryStream();
        using var writer = new StreamWriter(memoryStream) { AutoFlush = true };

        // Act & Assert
        Assert.Throws<ArgumentException>(() => 
            UserController.Register(writer, request, mockUserService.Object));
    }

    [Test]
    public void Register_MalformedJson_ThrowsJsonException()
    {
        // Arrange
        var request = new Request
        {
            Body = "{\"Username\":\"testuser\" \"Password\":\"123}" // Missing ,
        };
        var mockUserService = new Mock<IUserService>();
        using var memoryStream = new MemoryStream();
        using var writer = new StreamWriter(memoryStream) { AutoFlush = true };
        
        // Act & Assert
        Assert.Throws<JsonException>(() => 
            UserController.Register(writer, request, mockUserService.Object));
    }

    [Test]
    public void Register_InvalidJsonFormat_ThrowsJsonException()
    {
        // Arrange
        var request = new Request
        {
            Body = "Invalid JSON data"
        };
        var mockUserService = new Mock<IUserService>();
        using var memoryStream = new MemoryStream();
        using var writer = new StreamWriter(memoryStream) { AutoFlush = true };
        
        // Act & Assert
        Assert.Throws<JsonException>(() => 
            UserController.Register(writer, request, mockUserService.Object));
    }

    [Test]
    public void Register_EmptyUsername_ThrowsArgumentException()
    {
        // Arrange
        var request = new Request
        {
            Body = "{\"Username\":\"\", \"Password\":\"123\"}"
        };
        var mockUserService = new Mock<IUserService>();
        mockUserService
            .Setup(us => us.RegisterUser(It.Is<User>(u => string.IsNullOrEmpty(u.Username))))
            .Throws(new ArgumentException("Username or password is empty"));
        
        using var memoryStream = new MemoryStream();
        using var writer = new StreamWriter(memoryStream) { AutoFlush = true };
        
        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            UserController.Register(writer, request, mockUserService.Object));
    }

    [Test]
    public void Register_EmptyPassword_ThrowsArgumentException()
    {
        // Arrange
        var request = new Request
        {
            Body = "{\"Username\":\"testuser\", \"Password\":\"\"}"
        };
        var mockUserService = new Mock<IUserService>();
        mockUserService
            .Setup(us => us.RegisterUser(It.Is<User>(u => string.IsNullOrEmpty(u.Password))))
            .Throws(new ArgumentException("Username or password is empty"));
        
        using var memoryStream = new MemoryStream();
        using var writer = new StreamWriter(memoryStream) { AutoFlush = true };
        
        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            UserController.Register(writer, request, mockUserService.Object));
    }

    [Test]
    public void Register_MissingPassword_ThrowsArgumentException()
    {
        // Arrange
        var request = new Request
        {
            Body = "{\"Username\":\"testuser\"}" // Missing Password field
        };
        var mockUserService = new Mock<IUserService>();
        mockUserService
            .Setup(us => us.RegisterUser(It.Is<User>(u => 
                u.Username == "testuser" && string.IsNullOrEmpty(u.Password))))
            .Throws(new ArgumentException("Username or password is empty"));

        using var memoryStream = new MemoryStream();
        using var writer = new StreamWriter(memoryStream) { AutoFlush = true };
        
        // Act & Assert
        Assert.Throws<ArgumentException>(() => 
            UserController.Register(writer, request, mockUserService.Object));
    }

    [Test]
    public void Register_AdditionalFields_IgnoresExtraFields()
    {
        // Arrange
        var request = new Request
        {
            Body = "{\"Username\":\"testuser\", \"Password\":\"123\", \"ExtraField\":\"value\"}"
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
        mockUserService.Verify(us => us.RegisterUser(It.Is<User>(u => 
            u.Username == "testuser" && u.Password == "123")), Times.Once);
    }

    [Test]
    public void Register_RepositoryFailure_PropagatesException()
    {
        // Arrange
        var request = new Request
        {
            Body = "{\"Username\":\"testuser\", \"Password\":\"123\"}"
        };
        
        var mockUserService = new Mock<IUserService>();
        mockUserService
            .Setup(us => us.RegisterUser(It.IsAny<User>()))
            .Throws(new Exception("Database connection failed"));
        
        using var memoryStream = new MemoryStream();
        using var writer = new StreamWriter(memoryStream) { AutoFlush = true };
        
        // Act & Assert
        Assert.Throws<Exception>(() => 
            UserController.Register(writer, request, mockUserService.Object));
    }
}