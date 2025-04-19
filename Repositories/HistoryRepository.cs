using System.Data;
using Npgsql;
using SEB.Models;

namespace SEB.Repositories
{

    public class HistoryRepository(string connectionString)
    {
        public static void AddParameterWithValue(IDbCommand command, string parameterName, DbType type, object value)
        {
            var parameter = command.CreateParameter();
            parameter.DbType = type;
            parameter.ParameterName = parameterName;
            parameter.Value = value ?? DBNull.Value;
            command.Parameters.Add(parameter);
        }

        public List<History> GetUserHistory(int userId)
        {
            List<History> historyEntries = new List<History>();

            using IDbConnection connection = new NpgsqlConnection(connectionString);
            connection.Open();

            using IDbCommand command = connection.CreateCommand();
            command.CommandText = @"
                SELECT id, user_id, pushup_count, duration, timestamp 
                FROM history 
                WHERE user_id = @userId
                ORDER BY timestamp DESC";
            
            AddParameterWithValue(command, "userId", DbType.Int32, userId);

            using IDataReader reader = command.ExecuteReader();
            while(reader.Read())
            {
                historyEntries.Add(new History
                {
                    Id = reader.GetInt32(0),
                    UserId = reader.GetInt32(1),
                    PushupCount = reader.GetInt32(2),
                    Duration = reader.GetInt32(3),
                    Timestamp = reader.GetDateTime(4)
                });
            }
            
            return historyEntries;
        }
    }
}