using SEB.Http;

namespace SEB
{
    public class Program
    {
        public static void Main(string[] args)
        {
            int port = AppConfig.ServerPort;
            Http.Server server = new Http.Server(port);
            server.Start();
        }
    }
}