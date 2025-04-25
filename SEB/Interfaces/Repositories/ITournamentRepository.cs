using SEB.Models;

namespace SEB.Interfaces;

public interface ITournamentRepository
{
    Tournament? GetCurrentTournament();
}
