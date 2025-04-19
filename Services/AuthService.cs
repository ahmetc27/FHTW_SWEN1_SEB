using SEB.Http;
using SEB.Repositories;

namespace SEB.Services
{
    public class AuthService
    {
        private readonly SessionRepository _sessionRepository;
    
        public AuthService(SessionRepository sessionRepository)
        {
            _sessionRepository = sessionRepository;
        }

        public bool IsAuthorized(Request request, Response response, out string username)
        {
            // out -> additional return value
            username = "";

            if(!request.Headers.ContainsKey("Authorization")) return false;    
            
            string authHeader = request.Headers["Authorization"];
            string receivedToken = authHeader.Replace("Basic ", "").Trim();
            
            if(!_sessionRepository.ExistToken(receivedToken)) return false;
                
            username = _sessionRepository.GetUsernameByToken(receivedToken);
            return true;
        }
    }
}