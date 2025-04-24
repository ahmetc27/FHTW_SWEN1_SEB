using System.Data;
using Npgsql;
using SEB.Interfaces;
using SEB.Models;
using SEB.Utils;

namespace SEB.Repositories;

public class HistoryRepository : BaseRepository, IHistoryRepository
{
    public History? GetHistoryByUserId(int id)
    {
        using IDbConnection connection = new NpgsqlConnection(connectionString);
        connection.Open();

        using IDbCommand command = connection.CreateCommand();

        command.CommandText =
            "SELECT count, duration FROM history WHERE user_id = @id";

        AddParameterWithValue(command, "@id", DbType.Int32, id);

        IDataReader reader = command.ExecuteReader();

        if(reader.Read())
        {
            History history = new History()
            {
                Count = reader.IsDBNull(2) ? 0 : reader.GetInt32(2),
                Duration = reader.IsDBNull(3) ? 0 : reader.GetInt32(3)
            };
            Logger.Info($"Count: {history.Count}, Duration: {history.Duration}");
            return history;
        }
        return null;
    }
}