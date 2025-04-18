using SEB.Models;
using SEB.Http;

namespace SEB
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Server server = new Server(10001);
            server.Start();
        }
    }
}

// ERLEDIGT! TOKEN: muss in datenbank gespeichert werden
// ERLEDIGT! TOKEN: passwort muss auch überprüft werden
// (BaseRepository erstellen)
// ((exceptions try catch))

//17.04 letzer stand: wieso curl bat 3) nicht geht und put users (profile edit)