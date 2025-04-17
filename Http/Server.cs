using System.Net;
using System.Net.Sockets;

namespace SEB.Http
{
    public class Server
    {
        private TcpListener tcpListener;
        private readonly ServerService service;
        public Server(int port)
        {
            tcpListener = new TcpListener(IPAddress.Any, port);
            service = new ServerService();
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
                using StreamWriter writer = new StreamWriter(stream);

                service.ParseRequestLine(reader, writer);
                service.ParseHeaders(reader, writer);
                service.ParseBody(reader, writer);
            }
        }
    }
}