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

public class TournamentTests
{

    [Test]
    public void GetCurrentTournament_WithActiveTournament_ReturnsTournament()
    {
        // Arrange
        var token = "test-sebToken";

        var activeTournament = new Tournament
        {
            Id = 1,
            StartTime = DateTime.UtcNow.AddMinutes(-1), // Started 1 minute ago
            Status = "active",
            Winner = null
        };

        Mock<IUserRepository> mockUserRepo = new Mock<IUserRepository>();
        Mock<ISessionRepository> mockSessionRepo = new Mock<ISessionRepository>();
        Mock<ITournamentRepository> mockTournamentRepo = new Mock<ITournamentRepository>();

        mockSessionRepo.Setup(repo => repo.ExistToken(token))
            .Returns(true);

        mockTournamentRepo.Setup(repo => repo.GetCurrentTournament())
            .Returns(activeTournament);

        var tournamentService = new TournamentService(mockUserRepo.Object, mockSessionRepo.Object, mockTournamentRepo.Object);

        // Act
        var result = tournamentService.GetCurrentTournament(token);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.ActiveTournament.Id, Is.EqualTo(1));
        Assert.That(result.ActiveTournament.Status, Is.EqualTo("active"));
        Assert.That(result.PastTournaments, Is.Null);

        mockSessionRepo.Verify(repo => repo.ExistToken(token), Times.Once);
        mockTournamentRepo.Verify(repo => repo.GetCurrentTournament(), Times.Once);
    }

    [Test]
    public void GetCurrentTournament_PassiveTournament_ReturnsPastTournaments()
    {
        // Arrange
        var token = "test-sebToken";

        var tournaments = new List<Tournament>
        {
            new Tournament
            {
                Id = 1,
                Status = "ended"
            }
        };

        Mock<IUserRepository> mockUserRepo = new Mock<IUserRepository>();
        Mock<ISessionRepository> mockSessionRepo = new Mock<ISessionRepository>();
        Mock<ITournamentRepository> mockTournamentRepo = new Mock<ITournamentRepository>();

        mockSessionRepo.Setup(repo => repo.ExistToken(token))
            .Returns(true);

        mockTournamentRepo.Setup(repo => repo.GetCurrentTournament())
            .Returns((Tournament)null);

        mockTournamentRepo.Setup(repo => repo.GetAllTournaments())
            .Returns(tournaments);

        var tournamentService = new TournamentService(mockUserRepo.Object, mockSessionRepo.Object, mockTournamentRepo.Object);

        // Act
        var result = tournamentService.GetCurrentTournament(token);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.ActiveTournament, Is.Null);
        Assert.That(result.PastTournaments, Is.Not.Null);

        mockSessionRepo.Verify(repo => repo.ExistToken(token), Times.Once);
        mockTournamentRepo.Verify(repo => repo.GetCurrentTournament(), Times.Once);
    }
}