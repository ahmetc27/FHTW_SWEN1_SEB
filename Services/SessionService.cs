using SEB.Models;
using SEB.Http;
using SEB.Repositories;
using System.Text.Json;

namespace SEB.Services
{
    public class SessionService
    {
        private UserRepository userRepository = new UserRepository();
        private SessionRepository sessionRepository = new SessionRepository();
        private Response response = new Response();
        public void PostSessions(StreamWriter writer, Request request)
        {
            User? loginRequest = JsonSerializer.Deserialize<User>(request.Body);
            if(loginRequest == null)
            {
                response.SendNotFound(writer, "Invalid request body");
                return;
            }

            User? userInDb = userRepository.GetUser(loginRequest.Username);
            if(userInDb == null)
            {
                response.SendNotFound(writer, "User not found");
                return;
            }

            if(userInDb.Password != loginRequest.Password)
            {
                response.SendUnauthorized(writer, "Incorrect Password");
                return;
            }

            userInDb.Token = $"{userInDb.Username}-sebToken";

            if(sessionRepository.SaveToken(userInDb))
            {
                response.SendCreated(writer, "Token created successfully!");
            }
            else
            {
                response.SendInternalError(writer, "Token could not be saved");
            }
        }
    }
}