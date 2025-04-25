using System.Data;
using Npgsql;
using SEB.Interfaces;
using SEB.Models;

namespace SEB.Repositories;

public class TournamentRepository : BaseRepository, ITournamentRepository
{
    public Tournament? GetCurrentTournament()
    {
        using IDbConnection connection = new NpgsqlConnection(connectionString);
        connection.Open();
        
        using IDbCommand command = connection.CreateCommand();
        command.CommandText = "SELECT id, start_time, end_time, status FROM tournaments WHERE status=@status";
        AddParameterWithValue(command, "@status", DbType.String, "active");

        using IDataReader reader = command.ExecuteReader();
        
        if(reader.Read())
        {
            return new Tournament
            {
                Id = reader.GetInt32(0),
                StartTime = reader.GetDateTime(1),
                EndTime = reader.IsDBNull(2) ? null : reader.GetDateTime(2),
                Status = reader.GetString(3)
            };
        }
        return null;
    }
}