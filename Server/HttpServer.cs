using System.Net;
using System.Net.Sockets;
using SEB.Services;

namespace SEB.Server
{
    public class HttpServer
    {
        private TcpListener tcpListener;
        private readonly ServerService service;
        public HttpServer(int port)
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
                using StreamWriter writer = new StreamWriter(stream) { AutoFlush = true };

                service.ParseRequestLine(reader, writer);
                service.ParseHeaders(reader, writer);
                if(service.MethodHasBody()) service.ParseBody(reader, writer);

                service.RouteRequest(reader, writer);
            }
        }
    }
}