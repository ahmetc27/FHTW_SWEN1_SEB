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

public class StatsTests
{

    [Test]
    public void GetStats_WithValidToken_ReturnsStats()
    {
        // Arrange
        var token = "test-sebToken";

        var stats = new Stats
        {
            Id = 1,
            Elo = 110,
            OverallPushups = 150
        };

        var mockSessionRepo = new Mock<ISessionRepository>();
        var mockStatsRepo = new Mock<IStatsRepository>();

        mockSessionRepo.Setup(repo => repo.ExistToken(token))
            .Returns(true);

        mockStatsRepo.Setup(repo => repo.GetUserStatsByToken(token))
            .Returns((1, 110, 150));

        var statsService = new StatsService(mockSessionRepo.Object, mockStatsRepo.Object);

        // Act
        var result = statsService.GetStatistics(token);

        // Assert
        Assert.That(result.Id, Is.EqualTo(stats.Id));
        Assert.That(result.Elo, Is.EqualTo(stats.Elo));
        Assert.That(result.OverallPushups, Is.EqualTo(stats.OverallPushups));

        mockSessionRepo.Verify(repo => repo.ExistToken(token), Times.Once);
        mockStatsRepo.Verify(repo => repo.GetUserStatsByToken(token), Times.Once);
    }

    [Test]
    public void GetAllStats_WithInvalidToken_ThrowsUnauthorizedException()
    {
        // Arrange
        var token = "invalid-token";
        
        var mockSessionRepo = new Mock<ISessionRepository>();
        var mockStatsRepo = new Mock<IStatsRepository>();
        
        mockSessionRepo.Setup(repo => repo.ExistToken(token))
            .Returns(false);
        
        var statsService = new StatsService(mockSessionRepo.Object, mockStatsRepo.Object);

        // Act & Assert
        var ex = Assert.Throws<UnauthorizedException>(() => 
            statsService.GetStatistics(token));
            
        Assert.That(ex.Message, Is.EqualTo("Token does not exist"));
        
        mockSessionRepo.Verify(repo => repo.ExistToken(token), Times.Once);
        mockStatsRepo.Verify(repo => repo.GetUserStatsByToken(It.IsAny<string>()), Times.Never);
    }

    [Test]
    public void GetStats_WhenEloAbove110_ReturnsRankDiamond()
    {
        // Arrange
        var expectedStats = new Stats()
        {
            Id = 1,
            Elo = 120,
            OverallPushups = 500,
            Rank = "Diamond"
        };

        var token = "test-sebToken";

        var mockSessionRepo = new Mock<ISessionRepository>();
        var mockStatsRepo = new Mock<IStatsRepository>();

        mockSessionRepo.Setup(repo => repo.ExistToken(token))
            .Returns(true);    

        mockStatsRepo.Setup(repo => repo.GetUserStatsByToken(token))
            .Returns((1, 120, 500));

        var statsService = new StatsService(mockSessionRepo.Object, mockStatsRepo.Object);

        // Act
        var result = statsService.GetStatistics(token);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Rank, Is.EqualTo("Diamond"));
    }
}