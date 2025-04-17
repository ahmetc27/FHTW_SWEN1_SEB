using SEB.Models;
using System.Data;
using Npgsql;

namespace SEB.Repositories
{
    public class SessionRepository(string connectionString)
    {
        public static void AddParameterWithValue(IDbCommand command, string parameterName, DbType type, object value)
        {
            var parameter = command.CreateParameter();
            parameter.DbType = type;
            parameter.ParameterName = parameterName;
            parameter.Value = value ?? DBNull.Value;
            command.Parameters.Add(parameter);
        }
        public bool CreateToken(User user)
        {
            using IDbConnection connection = new NpgsqlConnection(connectionString);
            using IDbCommand command = connection.CreateCommand();
            connection.Open();

            command.CommandText = "UPDATE users SET token = @token WHERE username = @username AND password = @password";
            AddParameterWithValue(command, "token", DbType.String, user.Token);
            AddParameterWithValue(command, "username", DbType.String, user.Username);
            AddParameterWithValue(command, "password", DbType.String, user.Password);
            
            int rowsAffected = command.ExecuteNonQuery();
            if(rowsAffected == 0)
            {
                return false;
            }
            return true;
        }
    }
}