using Npgsql;
using SEB.Interfaces;
using SEB.Models;
using SEB.Utils;
using System.Data;

namespace SEB.Repositories;
public class StatsRepository : BaseRepository, IStatsRepository
{
    public (int userId, int elo, int totalPushups)? GetUserStatsByToken(string token)
    {
        using IDbConnection connection = new NpgsqlConnection(connectionString);
        connection.Open();
        
        int? userId = null;
        int? elo = null;

        using(IDbCommand command = connection.CreateCommand())
        {
            command.CommandText = "SELECT id, elo FROM users WHERE token=@token";
            AddParameterWithValue(command, "@token", DbType.String, token);

            using IDataReader reader = command.ExecuteReader();
            if(reader.Read())
            {
                userId = reader.GetInt32(0);
                elo = reader.GetInt32(1);
            }
        }

        if(userId == null || elo == null) return null;


        int totalPushups = 0;
        using(IDbCommand command2 = connection.CreateCommand())
        {
            command2.CommandText = "SELECT SUM(count) FROM history WHERE user_id=@userId";
            AddParameterWithValue(command2, "@userId", DbType.Int32, userId);

            using IDataReader reader = command2.ExecuteReader();
            if(reader.Read())
            {
                if(!reader.IsDBNull(0))
                    totalPushups = reader.GetInt32(0);
            }
        }

        return (userId.Value, elo.Value, totalPushups);
    }

    public List<Stats> GetAllStats()
    {
        using IDbConnection connection = new NpgsqlConnection(connectionString);
        connection.Open();
        
        using IDbCommand command = connection.CreateCommand();
        command.CommandText =
            "SELECT u.username, u.elo, COALESCE(SUM(h.count), 0) AS totalPushups " +
            "FROM users u " +
            "LEFT JOIN history h ON u.id = h.user_id " +
            "GROUP BY u.id";

        using IDataReader reader = command.ExecuteReader();

        List<Stats> scoreboard = new();
        
        while(reader.Read())
        {
            Stats stats = new Stats()
            {
                Id = reader.GetInt32(0),
                Elo = reader.GetInt32(1),
                OverallPushups = reader.GetInt32(2)
            };
            scoreboard.Add(stats);
        }
        return scoreboard;
    }
}