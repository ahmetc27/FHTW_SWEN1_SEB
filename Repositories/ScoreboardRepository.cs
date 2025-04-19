using System.Data;
using Npgsql;
using SEB.Models;

namespace SEB.Repositories
{

    public class ScoreboardRepository(string connectionString)
    {
        public static void AddParameterWithValue(IDbCommand command, string parameterName, DbType type, object value)
        {
            var parameter = command.CreateParameter();
            parameter.DbType = type;
            parameter.ParameterName = parameterName;
            parameter.Value = value ?? DBNull.Value;
            command.Parameters.Add(parameter);
        }
        public List<UserStats> GetScoreboard()
        {
            List<UserStats> scoreboard = new List<UserStats>();
            
            using IDbConnection connection = new NpgsqlConnection(connectionString);
            connection.Open();

            using IDbCommand command = connection.CreateCommand();
            // Join users and history tables to get username, elo, and sum of push-ups

            command.CommandText = @"
                SELECT u.username, u.elo, COALESCE(SUM(h.pushup_count), 0) as total_pushups
                FROM users u
                LEFT JOIN history h ON u.id = h.user_id
                GROUP BY u.username, u.elo
                ORDER BY u.elo DESC, total_pushups DESC";

            using IDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                scoreboard.Add(new UserStats
                {
                    Username = reader.GetString(0),
                    Elo = reader.GetInt32(1),
                    TotalPushups = reader.GetInt32(2)
                });
            }
            
            return scoreboard;
        }
    }
}