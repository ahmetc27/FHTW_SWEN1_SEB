using SEB.Models;

namespace SEB.Interfaces;

public interface ITournamentService
{
    Tournament? GetCurrentTournament(string token);
    void EvaluateTournament(int tournamentId);
}