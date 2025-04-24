using System.Data;
using Npgsql;
using SEB.Interfaces;
using SEB.Models;
using SEB.Utils;

namespace SEB.Repositories;

public class HistoryRepository : BaseRepository, IHistoryRepository
{
    public History? GetHistoryByUserId(int userid)
    {
        using IDbConnection connection = new NpgsqlConnection(connectionString);
        connection.Open();

        using IDbCommand command = connection.CreateCommand();

        command.CommandText =
            "SELECT count, duration FROM history WHERE user_id = @userid";

        AddParameterWithValue(command, "@userid", DbType.Int32, userid);

        IDataReader reader = command.ExecuteReader();

        if(reader.Read())
        {
            History history = new History()
            {
                Count = reader.IsDBNull(0) ? 0 : reader.GetInt32(0),
                Duration = reader.IsDBNull(1) ? 0 : reader.GetInt32(1)
            };
            Logger.Info($"Count: {history.Count}, Duration: {history.Duration}");
            return history;
        }
        return null;
    }
}