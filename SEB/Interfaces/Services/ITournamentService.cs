using SEB.DTOs;

namespace SEB.Interfaces;

public interface ITournamentService
{
    TournamentResponse GetCurrentTournament(string token);
    void EvaluateTournament(int tournamentId);
}