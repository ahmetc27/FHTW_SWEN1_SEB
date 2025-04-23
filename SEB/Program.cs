using SEB.Http;
using SEB.Interfaces;
using SEB.Repositories;
using SEB.Services;

IServerService _serverService = new ServerService();
IUserRepository _userRepository = new UserRepository();
IUserService _userService = new UserService(_userRepository);
Router _router = new(_userService);

Server server = new Server(10001, _serverService, _router);
server.Start();