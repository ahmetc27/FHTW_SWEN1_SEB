using SEB.Models;
using SEB.Http;
using SEB.Repositories;
using System.Text.Json;

namespace SEB.Service
{
    public class UserService
    {
        private UserRepository userRepository = new UserRepository("Host=localhost;Username=postgres;Password=postgres;Database=postgres");
        public void PostUser(StreamWriter writer, Request request)
        {
            User? user = JsonSerializer.Deserialize<User>(request.Body);
            if(user == null) return; // muss mit return genauer was gemacht werden

            userRepository.Add(user);
        }
    }
}