using SEB.Server;

namespace SEB
{
    public class Program
    {
        public static void Main(string[] args)
        {
            HttpServer server = new HttpServer(10001);
            server.Start();
        }
    }
}