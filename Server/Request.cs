namespace SEB.Server
{
    public class Request
    {
        public string Method { get; set; } = "";
        public string Path { get; set; } = "";
        public string Version { get; set; } = "";
        public Dictionary<string, string> Headers = new Dictionary<string, string>();
        public int ContentLength { get; set; }
        public string Body { get; set; } = "";
    }
}