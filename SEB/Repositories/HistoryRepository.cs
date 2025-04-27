using System.Data;
using Npgsql;
using SEB.Interfaces;
using SEB.Models;
using SEB.Utils;

namespace SEB.Repositories;

public class HistoryRepository : BaseRepository, IHistoryRepository
{
    public List<History>? GetHistoryByUserId(int userid)
    {
        using IDbConnection connection = new NpgsqlConnection(connectionString);
        connection.Open();

        using IDbCommand command = connection.CreateCommand();

        command.CommandText =
            "SELECT id, count, duration FROM history WHERE user_id = @userid";

        AddParameterWithValue(command, "@userid", DbType.Int32, userid);

        IDataReader reader = command.ExecuteReader();

        List<History> historyEntries = new();

        while(reader.Read())
        {
            History history = new History
            {
                Id = reader.IsDBNull(0) ? 0 : reader.GetInt32(0),
                Count = reader.IsDBNull(1) ? 0 : reader.GetInt32(1),
                Duration = reader.IsDBNull(2) ? 0 : reader.GetInt32(2),
            };

            historyEntries.Add(history);
        }
        return historyEntries != null ? historyEntries : null;
    }

    public History Add(int userid, History history)
    {
        using IDbConnection connection = new NpgsqlConnection(connectionString);
        connection.Open();

        using IDbCommand command = connection.CreateCommand();

        command.CommandText =
            "INSERT INTO history (user_id, name, count, duration, tournament_id) VALUES (@userid, @name, @count, @duration, @tournament_id) RETURNING name, count, duration";

        AddParameterWithValue(command, "@userid", DbType.Int32, userid);
        AddParameterWithValue(command, "@name", DbType.String, history.Name);
        AddParameterWithValue(command, "@count", DbType.Int32, history.Count);
        AddParameterWithValue(command, "@duration", DbType.Int32, history.Duration);
        AddParameterWithValue(command, "@tournament_id", DbType.Int32, history.TournamentId);

        using IDataReader reader = command.ExecuteReader();
        if(reader.Read())
        {
            return new History
            {
                Name = reader.GetString(0),
                Count = reader.GetInt32(1),
                Duration = reader.GetInt32(2)
            };
        }

        throw new Exception(ErrorMessages.DatabaseInsertHistoryError);
    }
}