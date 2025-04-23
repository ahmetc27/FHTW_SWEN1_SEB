using SEB.Http;
using SEB.Interfaces;
using SEB.Repositories;
using SEB.Services;

IUserRepository userRepository = new UserRepository();
ISessionRepository sessionRepository = new SessionRepository();
IStatsRepository statsRepository = new StatsRepository();

IUserService userService = new UserService(userRepository);
ISessionService sessionService = new SessionService(userRepository, sessionRepository);
IStatsService statsService = new StatsService(userRepository, sessionRepository, statsRepository);

IServerService serverService = new ServerService();
Router router = new(userService, sessionService, statsService);

Server server = new Server(10001, serverService, router);
server.Start();