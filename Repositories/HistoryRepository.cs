using System.Data;
using Npgsql;
using SEB.Models;

namespace SEB.Repositories
{

    public class HistoryRepository
    {
        private readonly string connectionString;

        public HistoryRepository()
        {
            connectionString = AppConfig.ConnectionString;
        }
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

        public int AddHistoryEntry(int userId, int pushupCount, int durationInSeconds, int? tournamentId = null)
        {
            using IDbConnection connection = new NpgsqlConnection(connectionString);
            connection.Open();

            using IDbCommand command = connection.CreateCommand();
            command.CommandText = @"
                INSERT INTO history (user_id, pushup_count, duration, tournament_id)
                VALUES (@userId, @pushupCount, @durationInSeconds, @tournamentId)
                RETURNING id";
                
            AddParameterWithValue(command, "userId", DbType.Int32, userId);
            AddParameterWithValue(command, "pushupCount", DbType.Int32, pushupCount);
            AddParameterWithValue(command, "durationInSeconds", DbType.Int32, durationInSeconds);
            AddParameterWithValue(command, "tournamentId", DbType.Int32, tournamentId.HasValue ? tournamentId.Value : DBNull.Value);
            
            int historyId = Convert.ToInt32(command.ExecuteScalar());
            return historyId;
        }
    }
}