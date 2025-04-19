using SEB.Server;

namespace SEB
{
    public class Program
    {
        public static void Main(string[] args)
        {
            int port = AppConfig.ServerPort;
            HttpServer server = new HttpServer(port);
            server.Start();
        }
    }
}