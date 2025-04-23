using Npgsql;
using SEB.Interfaces;
using SEB.Models;
using System.Data;

namespace SEB.Repositories;
public class StatsRepository : BaseRepository, IStatsRepository
{
    public int? GetEloByToken(string token)
    {
        using IDbConnection connection = new NpgsqlConnection(connectionString);
        connection.Open();
        
        using IDbCommand command = connection.CreateCommand();
        command.CommandText = "SELECT elo FROM users WHERE token=@token";
        AddParameterWithValue(command, "@token", DbType.String, token);

        using IDataReader reader = command.ExecuteReader();
        if(reader.Read())
            return reader.GetInt32(0);

        return null;
    }

    public int? GetTotalPushupsById(int userId)
    {
        using IDbConnection connection = new NpgsqlConnection(connectionString);
        connection.Open();
        
        using IDbCommand command = connection.CreateCommand();
        command.CommandText = "SELECT SUM(count) FROM history WHERE user_id=@userId";
        AddParameterWithValue(command, "@userId", DbType.Int32, userId);

        using IDataReader reader = command.ExecuteReader();

        if(reader.Read() && !reader.IsDBNull(0))
            return reader.GetInt32(0);
        
        return 0;
    }
}