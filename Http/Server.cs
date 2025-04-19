using System.Net;
using System.Net.Sockets;
using SEB.Services;

namespace SEB.Http
{
    public class Server
    {
        private TcpListener tcpListener;
        private readonly ServerService serverService = new();
        private readonly Request request = new();
        private readonly Router router = new();
        public Server(int port)
        {
            tcpListener = new TcpListener(IPAddress.Any, port);
        }
        public void Start()
        {
            tcpListener.Start();
            Console.WriteLine("Server started. Waiting for connections...");

            while(true)
            {
                TcpClient tcpClient = tcpListener.AcceptTcpClient();

                using NetworkStream stream = tcpClient.GetStream();
                using StreamReader reader = new StreamReader(stream);
                using StreamWriter writer = new StreamWriter(stream) { AutoFlush = true };

                serverService.ParseRequest(reader, request);
                router.Route(writer, request);
            }
        }
    }
}