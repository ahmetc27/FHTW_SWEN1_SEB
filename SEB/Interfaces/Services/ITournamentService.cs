using SEB.Models;

namespace SEB.Interfaces;

public interface ITournamentService
{
    public Tournament? GetCurrentTournament(string token);
}