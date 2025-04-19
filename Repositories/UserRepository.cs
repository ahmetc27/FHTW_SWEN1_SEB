using SEB.Models;
using System.Data;
using Npgsql;

namespace SEB.Repositories
{
    public class UserRepository : BaseRepository
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

            command.CommandText = @"SELECT * FROM users WHERE username = @username;";
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
                    Token = reader.IsDBNull(4) ? "" : reader.GetString(4),
                    Bio = reader.IsDBNull(5) ? "" : reader.GetString(5),
                    Image = reader.IsDBNull(6) ? "" : reader.GetString(6)
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
                    Elo = reader.GetInt32(3),
                    Token = reader.IsDBNull(4) ? "" : reader.GetString(4),
                    Bio = reader.IsDBNull(5) ? "" : reader.GetString(5),
                    Image = reader.IsDBNull(6) ? "" : reader.GetString(6)
                });
            }
            return users;
        }

        public void Update(User user)
        {
            using IDbConnection connection = new NpgsqlConnection(connectionString);
            using IDbCommand command = connection.CreateCommand();
            connection.Open();

            command.CommandText = "UPDATE users SET username=@username, password=@password, elo=@elo, token=@token, bio=@bio, image=@image WHERE id=@id";
            AddParameterWithValue(command, "id", DbType.Int32, user.Id);
            AddParameterWithValue(command, "username", DbType.String, user.Username);
            AddParameterWithValue(command, "password", DbType.String, user.Password);
            AddParameterWithValue(command, "elo", DbType.Int32, user.Elo);
            AddParameterWithValue(command, "token", DbType.String, user.Token);
            AddParameterWithValue(command, "bio", DbType.String, user.Bio);
            AddParameterWithValue(command, "image", DbType.String, user.Image);
            command.ExecuteNonQuery();
        }
    }
}