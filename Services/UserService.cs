using SEB.Models;
using SEB.Http;
using SEB.Repositories;
using System.Text.Json;

namespace SEB.Services
{
    public class UserService
    {
        private UserRepository userRepository = new UserRepository();
        private SessionRepository sessionRepository = new SessionRepository();
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
            if(!request.Headers.ContainsKey("Authorization"))
            {
                response.SendUnauthorized(writer, "Authorization header required");
                return;
            }

            string authHeader = request.Headers["Authorization"];
            string receivedToken = authHeader.Replace("Basic ", "").Trim();

            string[] arr = request.Path.Split('/'); // /users/test2

            if(arr.Length < 3)
            {
                response.SendBadRequest(writer, "Invalid path");
                return;
            }
            string username = arr[2];

            User? userInDb = userRepository.GetUser(username);

            if(userInDb == null)
            {
                response.SendNotFound(writer, "User not found");
                return;
            }

            if(userInDb.Token != receivedToken || !sessionRepository.ExistToken(receivedToken))
            {
                response.SendUnauthorized(writer, "Invalid authentication token");
                return;
            }

            Dictionary<string, string>? userDetails = JsonSerializer.Deserialize<Dictionary<string, string>>(request.Body);

            if(userDetails != null)
            {
                if(userDetails.ContainsKey("Password")) userInDb.Password = userDetails["Password"];
                if(userDetails.ContainsKey("Elo")) userInDb.Elo = int.Parse(userDetails["Elo"]);
                if(userDetails.ContainsKey("Token")) userInDb.Token = userDetails["Token"];
                if(userDetails.ContainsKey("Bio")) userInDb.Bio = userDetails["Bio"];
                if(userDetails.ContainsKey("Image")) userInDb.Image = userDetails["Image"];

                userRepository.Update(userInDb);
                response.SendOk(writer, "User updated successfully!");
            }
            else
            {
                response.SendBadRequest(writer, "Invalid user details in request body");
                return;
            }
        }
    }
}