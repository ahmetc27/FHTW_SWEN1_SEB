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