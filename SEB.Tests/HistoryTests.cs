using NUnit.Framework;
using Moq;
using SEB.Models;
using SEB.Http;
using SEB.Controller;
using SEB.Interfaces;
using SEB.DTOs;
using SEB.Services;
using SEB.Exceptions;
using System.Text.Json;

namespace SEB.Tests;

public class HistoryTests
{

    [Test]
    public void GetHistory_WithValidToken_ReturnsHistory()
    {
        // Arrange
        var user = new User
        {
            UserId = 1,
            Username = "test",
            Password = "123",
            Token = "test-sebToken"
        };

        var expectedHistory = new List<History>
        {
            new History
            {
                Id = 1,
                Name = "Pushups",
                Count = 30,
                Duration = 120,
                TournamentId = 1
            }
        };

        Mock<IUserRepository> mockUserRepo = new Mock<IUserRepository>();
        Mock<ISessionRepository> mockSessionRepo = new Mock<ISessionRepository>();
        Mock<IHistoryRepository> mockHistoryRepo = new Mock<IHistoryRepository>();
        Mock<ITournamentRepository> mockTournamentRepo = new Mock<ITournamentRepository>();

        mockSessionRepo.Setup(repo => repo.ExistToken(user.Token))
            .Returns(true);

        mockUserRepo.Setup(repo => repo.GetIdByToken(user.Token))
            .Returns(user.UserId);
        
        mockHistoryRepo.Setup(repo => repo.GetHistoryByUserId(user.UserId))
            .Returns(expectedHistory);

        var historyService = new HistoryService(mockUserRepo.Object, mockSessionRepo.Object, mockHistoryRepo.Object, mockTournamentRepo.Object);

        // Act
        var result = historyService.GetUserHistoryData(user.Token);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count, Is.EqualTo(1));
        Assert.That(result[0].Count, Is.EqualTo(30));
        Assert.That(result[0].Duration, Is.EqualTo(120));
        mockSessionRepo.Verify(repo => repo.ExistToken(user.Token), Times.Once);
        mockUserRepo.Verify(repo => repo.GetIdByToken(user.Token), Times.Once);
        mockHistoryRepo.Verify(repo => repo.GetHistoryByUserId(user.UserId), Times.Once);
    }

    [Test]
    public void LogPushups_WithNegativeCount_ThrowsBadRequestException()
    {
        // Arrange
        var historyRequest = new HistoryRequest
        {
            Name = "Pushups",
            Count = -100,
            DurationInSeconds = 120
        };

        var token = "test-sebToken";

        Mock<IUserRepository> mockUserRepo = new Mock<IUserRepository>();
        Mock<ISessionRepository> mockSessionRepo = new Mock<ISessionRepository>();
        Mock<IHistoryRepository> mockHistoryRepo = new Mock<IHistoryRepository>();
        Mock<ITournamentRepository> mockTournamentRepo = new Mock<ITournamentRepository>();

        var historyService = new HistoryService(mockUserRepo.Object, mockSessionRepo.Object, mockHistoryRepo.Object, mockTournamentRepo.Object);

        // Act & Assert
        Assert.Throws<BadRequestException>(() => historyService.LogPushups(token, historyRequest));
    }
}