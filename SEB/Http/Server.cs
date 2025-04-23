using SEB.Interfaces;
using System.Net;
using System.Net.Sockets;

namespace SEB.Http;
public class Server
{
    private readonly TcpListener _listener;
    private readonly IServerService _service;
    private Router _router;
    public Server(int port, IServerService service, Router router)
    {
        _listener = new TcpListener(IPAddress.Any, port);
        _service = service;
        _router = router;
    }

    public void Start()
    {
        _listener.Start();
        Console.WriteLine("Server started. Waiting for connections...");

        while(true)
        {
            TcpClient client = _listener.AcceptTcpClient();

            var stream = client.GetStream();
            using var reader = new StreamReader(stream);
            using var writer = new StreamWriter(stream) { AutoFlush = true };

            var request = new Request();
                
            _service.ParseRequest(reader, request);
            _router.Route(request, writer);
        }
    }
}