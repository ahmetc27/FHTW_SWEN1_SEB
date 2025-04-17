namespace SEB.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = "";
        public string Password { get; set; } = "";
        public string Token { get; set; } = "";
        public int Elo { get; set; }
        public string Bio { get; set; } = "";
        public string Image { get; set; } = "";
    }
}