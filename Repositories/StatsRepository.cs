using System.Data;
using Npgsql;

namespace SEB.Repositories
{
    public class StatsRepository(string connectionString)
    {
        public static void AddParameterWithValue(IDbCommand command, string parameterName, DbType type, object value)
        {
            var parameter = command.CreateParameter();
            parameter.DbType = type;
            parameter.ParameterName = parameterName;
            parameter.Value = value ?? DBNull.Value;
            command.Parameters.Add(parameter);
        }
        public (int Elo, int TotalPushups) GetStats(string username)
        {
            using IDbConnection connection = new NpgsqlConnection(connectionString);
            connection.Open();
            int elo = 0;
            int totalPushups = 0;

            using(IDbCommand command = connection.CreateCommand())
            {
                command.CommandText = "SELECT elo FROM users WHERE username = @username";
                AddParameterWithValue(command, "username", DbType.String, username);
                object? result = command.ExecuteScalar();
                elo = Convert.ToInt32(result);
            }

            int userId = 0;
            using(IDbCommand userIdCommand = connection.CreateCommand())
            {
                userIdCommand.CommandText = "SELECT id FROM users WHERE username = @username";
                AddParameterWithValue(userIdCommand, "username", DbType.String, username);
                object? userIdResult = userIdCommand.ExecuteScalar();
                if(userIdResult != null && userIdResult != DBNull.Value)
                {
                    userId = Convert.ToInt32(userIdResult);

                    using(IDbCommand pushupCommand = connection.CreateCommand())
                    {
                        pushupCommand.CommandText = "SELECT COALESCE(SUM(pushup_count), 0) FROM history WHERE user_id = @userId";
                        AddParameterWithValue(pushupCommand, "userId", DbType.Int32, userId);
                        object? pushupResult = pushupCommand.ExecuteScalar();
                        totalPushups = pushupResult != null && pushupResult != DBNull.Value ? Convert.ToInt32(pushupResult) : 0;
                    }
                }
            }

            return (elo, totalPushups);
        }
    }
}