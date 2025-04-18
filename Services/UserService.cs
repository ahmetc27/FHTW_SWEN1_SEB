using SEB.Models;
using SEB.Http;
using SEB.Repositories;
using System.Text.Json;

namespace SEB.Service
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
                response.SendBadRequest(writer, "User may be null");
                return;
            }
            if(sessionRepository.GetToken(user.Token))
            {
                string token = request.Headers["Authorization"];
                if(token.Contains($"{user.Token}"))
                {
                    string json = JsonSerializer.Serialize(user);
                    response.SendOk(writer, json);
                }
                else
                {
                    response.SendBadRequest(writer, "No valid token found in header");
                }
            }
        }

        public void GetAllUser(StreamWriter writer)
        {
            string json = JsonSerializer.Serialize(userRepository.GetAllUsers());
            response.SendOk(writer, json);
        }

        public void UpdateUser(StreamWriter writer, Request request)
        {
            User? user = JsonSerializer.Deserialize<User>(request.Body);
            if(user == null)
            {
                response.SendBadRequest(writer, "Invalid user data");
                return;
            }
            userRepository.Update(user);
            response.SendOk(writer, "User updated successfully!");
        }
    }
}