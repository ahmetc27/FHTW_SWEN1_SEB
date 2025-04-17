using SEB.Models;
using System.Data;
using Npgsql;
using System.Collections;

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

        public bool Exists(string username)
        {
            using IDbConnection connection = new NpgsqlConnection(connectionString);
            using IDbCommand command = connection.CreateCommand();
            connection.Open();

            command.CommandText = "SELECT COUNT(*) from users WHERE username=@username";
            AddParameterWithValue(command, "username", DbType.String, username);

            int count = Convert.ToInt32(command.ExecuteScalar());
            return count > 0;
        }

        public User? GetUser(string username)
        {
            using IDbConnection connection = new NpgsqlConnection(connectionString);
            using IDbCommand command = connection.CreateCommand();
            connection.Open();

            command.CommandText = @"SELECT id, username, password, elo, token FROM users WHERE username = @username;";
            AddParameterWithValue(command, "username", DbType.String, username);

            using IDataReader reader = command.ExecuteReader();
            if(reader.Read())
            {
                return new User()
                {
                    Id = reader.GetInt32(0),
                    Username = reader.GetString(1),
                    Password = reader.GetString(2),
                    Elo = reader.GetInt32(3),
                    Token = reader.GetString(4),
                };
            }
            return null;
        }

        public IEnumerable<User> GetAllUsers()
        {
            using IDbConnection connection = new NpgsqlConnection(connectionString);
            using IDbCommand command = connection.CreateCommand();
            connection.Open();

            command.CommandText = @"SELECT * FROM users";

            List<User> users = new List<User>();

            using IDataReader reader = command.ExecuteReader();
            while(reader.Read())
            {
                users.Add(new User()
                {
                    Id = reader.GetInt32(0),
                    Username = reader.GetString(1),
                    Password = reader.GetString(2),
                    Elo = reader.GetInt32(3)
                });
            }
            return users;
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