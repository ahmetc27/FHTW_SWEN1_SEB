using SEB.Http;
using SEB.Interfaces;
using SEB.Repositories;
using SEB.Services;

IUserRepository userRepository = new UserRepository();
ISessionRepository sessionRepository = new SessionRepository();

IUserService userService = new UserService(userRepository);
ISessionService sessionService = new SessionService(userRepository, sessionRepository);

IServerService _serverService = new ServerService();
Router _router = new(userService, sessionService);

Server server = new Server(10001, _serverService, _router);
server.Start();