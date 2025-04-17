using SEB.Models;
using System.Data;
using Npgsql;

namespace SEB.Repositories
{
    public class UserRepository(string connectionString)
    {
        public void Add(User user)
        {
            using IDbConnection connection = new NpgsqlConnection(connectionString);
            using IDbCommand command = connection.CreateCommand();
            connection.Open();

            command.CommandText = "INSERT INTO users (username, password) " +
                "VALUES (@username, @password) RETURNING id";
            AddParameterWithValue(command, "username", DbType.String, user.Username);
            AddParameterWithValue(command, "password", DbType.String, user.Password);
            user.Id = (int)(command.ExecuteScalar() ?? 0);
        }

        public bool CheckDuplicate(string username)
        {
            using IDbConnection connection = new NpgsqlConnection(connectionString);
            using IDbCommand command = connection.CreateCommand();
            connection.Open();

            command.CommandText = "SELECT COUNT(*) from users WHERE username=@username";
            AddParameterWithValue(command, "username", DbType.String, username);

            int count = Convert.ToInt32(command.ExecuteScalar());
            return count > 0;

            /*command.CommandText = "SELECT 1 FROM users WHERE username = @username LIMIT 1";
            AddParameterWithValue(command, "username", DbType.String, username);

            using IDataReader reader = command.ExecuteReader();
            return reader.Read(); // Returns true if a row exists, false otherwise*/

        }

        public static void AddParameterWithValue(IDbCommand command, string parameterName, DbType type, object value)
        {
            var parameter = command.CreateParameter();
            parameter.DbType = type;
            parameter.ParameterName = parameterName;
            parameter.Value = value ?? DBNull.Value;
            command.Parameters.Add(parameter);
        }
    }
}