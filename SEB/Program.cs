using SEB.Http;
using SEB.Interfaces;
using SEB.Repositories;
using SEB.Services;

IUserRepository userRepository = new UserRepository();
ISessionRepository sessionRepository = new SessionRepository();
IStatsRepository statsRepository = new StatsRepository();
IHistoryRepository historyRepository = new HistoryRepository();
ITournamentRepository tournamentRepository = new TournamentRepository();

IUserService userService = new UserService(userRepository);
ISessionService sessionService = new SessionService(userRepository, sessionRepository);
IStatsService statsService = new StatsService(sessionRepository, statsRepository);
IHistoryService historyService = new HistoryService(userRepository, sessionRepository, historyRepository, tournamentRepository);
ITournamentService tournamentService = new TournamentService(userRepository, sessionRepository, tournamentRepository);

IServerService serverService = new ServerService();
Router router = new(userService, sessionService, statsService, historyService, tournamentService);

Server server = new Server(10001, serverService, router);
server.Start();