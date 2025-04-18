using SEB.Models;
using SEB.Server;
using SEB.Repositories;
using System.Text.Json;

namespace SEB.Services
{
    public class UserService
    {
        private UserRepository userRepository = new UserRepository("Host=localhost;Username=postgres;Password=postgres;Database=postgres");
        private SessionRepository sessionRepository = new SessionRepository("Host=localhost;Username=postgres;Password=postgres;Database=postgres");
        private Response response = new Response();
        public void PostUser(StreamWriter writer, Request request)
        {
            User? user = JsonSerializer.Deserialize<User>(request.Body);
            if(user == null)
            {
                response.SendBadRequest(writer, "User must not be null");
                return;
            }

            if(!userRepository.Exists(user.Username))
            {
                userRepository.Add(user);
                response.SendCreated(writer, "User created successfully!");
            }
            else
            {
                response.SendBadRequest(writer, "Username already exists");
            }
        }

        public void GetUser(StreamWriter writer, Request request)
        {
            // extract path: /users/test1
            string username = "";
            string[] arr = request.Path.Split('/'); // users test1
            username = arr[2];

            User? user = userRepository.GetUser(username);
            if(user == null)
            {
                response.SendNotFound(writer, "User not found");
                return;
            }

            if(string.IsNullOrEmpty(user.Token))
            {
                response.SendUnauthorized(writer, "No active user token found");
                return;
            }

            if(!request.Headers.ContainsKey("Authorization"))
            {
                response.SendUnauthorized(writer, "Authorization header required");
                return;
            }

            string authHeader = request.Headers["Authorization"];
            string receivedToken = authHeader.Replace("Basic ", "").Trim();

            if(sessionRepository.ExistToken(user.Token) && user.Token == receivedToken)
            // test1 cannot send test2-token
            {
                string json = JsonSerializer.Serialize(user);
                response.SendOk(writer, json);
            }
            else
            {
                response.SendUnauthorized(writer, "Invalid authentication token");
            }
        }

        public void GetAllUser(StreamWriter writer)
        {
            try
            {
                string json = JsonSerializer.Serialize(userRepository.GetAllUsers());
                response.SendOk(writer, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetAllUser: {ex.Message}");
                response.SendInternalError(writer, $"An error occurred: {ex.Message}");
            }
        }

        public void UpdateUser(StreamWriter writer, Request request)
        {
            User? user = JsonSerializer.Deserialize<User>(request.Body);
            if(user == null)
            {
                response.SendBadRequest(writer, "Invalid user data");
                return;
            }

            if(string.IsNullOrEmpty(user.Token))
            {
                response.SendUnauthorized(writer, "No active user token found");
                return;
            }

            if(!request.Headers.ContainsKey("Authorization"))
            {
                response.SendUnauthorized(writer, "Authorization header required");
                return;
            }

            string authHeader = request.Headers["Authorization"];
            string receivedToken = authHeader.Replace("Basic ", "").Trim();

            if(sessionRepository.ExistToken(user.Token) && user.Token == receivedToken)
            {
                userRepository.Update(user);
                response.SendOk(writer, "User updated successfully!");
            }
            else
            {
                response.SendUnauthorized(writer, "Invalid authentication token");
            }
        }
    }
}