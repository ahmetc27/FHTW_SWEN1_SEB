using SEB.DTOs;
using SEB.Models;

namespace SEB.Interfaces;

public interface ITournamentRepository
{
    Tournament? GetCurrentTournament();
    Tournament StartNewTournament();
    TournamentParticipant? GetParticipant(int tId, int uId);
    void AddParticipant(int tournamentId, int userId, int count, int duration);
    void UpdateParticipant(int tournamentId, int userId, TournamentParticipant participant);
    public void EndTournament(int tournamentId);
    public List<TournamentParticipant> GetParticipants(int tournamentId);
    public List<Tournament>? GetAllTournaments();
}
