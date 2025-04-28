using SEB.Models;

namespace SEB.DTOs;

public class TournamentResponse
{
    public Tournament? ActiveTournament { get; set; }
    public List<Tournament>? PastTournaments { get; set; }
}