using SEB.Models;
using SEB.Http;
using SEB.Repositories;
using System.Text.Json;

namespace SEB.Service
{
    public class SessionsService
    {
        private UserRepository userRepository = new UserRepository("Host=localhost;Username=postgres;Password=postgres;Database=postgres");
        private SessionRepository tokenRepository = new SessionRepository("Host=localhost;Username=postgres;Password=postgres;Database=postgres");
        private Response response = new Response();
        public void PostSessions(StreamWriter writer, Request request)
        {
            User? user = JsonSerializer.Deserialize<User>(request.Body);
            if(user == null) return; // muss mit return genauer was gemacht werden

            if(userRepository.Exists(user.Username))
            {
                user.Token = $"{user.Username}-Token";
                if(tokenRepository.CreateToken(user))
                {
                    response.SendCreated(writer, "Token created successfully!");
                }
                else
                {
                    response.SendBadRequest(writer, "No user found with the given username and password.");
                }
            }
            else
            {
                response.SendBadRequest(writer, "Token cannot be created since user does not exist!");
            }
        }
    }
}