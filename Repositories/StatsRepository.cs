using System.Data;
using Npgsql;

namespace SEB.Repositories
{
    public class StatsRepository(string connectionString)
    {
        public void GetStats()
        {
            using IDbConnection connection = new NpgsqlConnection(connectionString);
            using IDbCommand command = connection.CreateCommand();
            connection.Open();

            command.CommandText = "SELECT elo FROM users WHERE username = @username; SELECT SUM(count) FROM pushups WHERE username = @username";

        }
    }
}