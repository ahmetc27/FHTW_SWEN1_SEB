using SEB.Models;
using SEB.Http;
using System.Text.Json;

namespace SEB.Service
{
    public class UserService
    {
        public void PostUser(StreamWriter writer, Request request)
        {
            User? user = JsonSerializer.Deserialize<User>(request.Body);
            if(user == null) return;

            Console.WriteLine($"User: {user.Username}, Pw: {user.Password}");

            //Weiterleiten an userrepository
        }
    }
}