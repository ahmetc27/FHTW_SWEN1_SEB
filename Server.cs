using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using Npgsql;

public class Server
{
    private readonly string connectionstring = "Host=localhost;Username=postgres;Password=postgres;Database=postgres";
    private readonly TcpListener tcpListener;
    private readonly UserRepository userRepository;
    public Server(int port)
    {
        tcpListener = new TcpListener(IPAddress.Any, port);
        userRepository = new UserRepository(connectionstring);
    }

    public void Start()
    {
        tcpListener.Start();
        Console.WriteLine("Server started. Waiting for connections...");
        HandleRequest();
    }

    public void HandleRequest()
    {
        while(true)
        {
            var client = tcpListener.AcceptTcpClient();
            using var writer = new StreamWriter(client.GetStream()) { AutoFlush = true };
            using var reader = new StreamReader(client.GetStream());

            // 1st line eg GET /users HTTP/1.1
            string? line;
            line = reader.ReadLine();
            if(line == null) return;
            var httpParts = line.Split(' ');
            if(httpParts.Length < 3) { Console.WriteLine("Something wrong"); return; }
            var method = httpParts[0];
            var path = httpParts[1];
            var version = httpParts[2];
            Console.WriteLine($"Method: {method}, Path: {path}, Version: {version}");

            // Headers
            int content_length = 0;
            while((line = reader.ReadLine()) != null)
            {
                if(line.Length == 0) break; 
                // emtpy line indicates the end of the HTTP-headers

                var headerParts = line.Split(':');
                var headerName = headerParts[0];
                var headerValue = headerParts[1].Trim();
                if(headerName == "Content-Length")
                {
                    content_length = int.Parse(headerValue);
                }
            }

            // Body
            StringBuilder requestBody = new StringBuilder();
            if(content_length > 0)
            {
                char[] chars = new char[1024];
                int bytesReadTotal = 0;
                while(bytesReadTotal < content_length)
                {
                    var bytesRead = reader.Read(chars, 0, chars.Length);
                    bytesReadTotal += bytesRead;
                    if(bytesRead == 0) { break; } // no more data available
                    requestBody.Append(chars, 0, bytesRead);
                }
            }
            string responseBody = "Initial responseBody";

            if(method == "POST" && path == "/users")
            {
                try
                {
                    string body = requestBody.ToString();
                    User? user = JsonSerializer.Deserialize<User>(body);
                    if(user == null)
                    {
                        responseBody = "User data is missing or invalid";
                        writer.WriteLine("HTTP/1.0 400 Bad Request");
                        writer.WriteLine("Content-Type: text/plain");
                        writer.WriteLine($"Content-Length: {responseBody.Length}");
                        writer.WriteLine();
                        writer.WriteLine(responseBody);
                        return;
                    }
                    userRepository.Add(user);
                    writer.WriteLine("HTTP/1.0 201 Created");
                    writer.WriteLine("Content-Type: text/plain");
                    writer.WriteLine($"Content-Length: {responseBody.Length}");
                    writer.WriteLine();
                    writer.WriteLine("User registered");
                }
                /*catch(Exception ex)
                {
                    throw new Exception(ex.Message);
                }*/
                catch(JsonException)
                {
                    responseBody = "Invalid JSON format";
                    writer.WriteLine("HTTP/1.0 400 Bad Request");
                    writer.WriteLine("Content-Type: text/plain");
                    writer.WriteLine($"Content-Length: {responseBody.Length}");
                    writer.WriteLine();
                    writer.WriteLine(responseBody);
                }
            }
            else if(method == "GET" && path == "/users")
            {
                IEnumerable<User> users = userRepository.GetAll();
                responseBody = JsonSerializer.Serialize(users);
                writer.WriteLine("HTTP/1.0 200 OK");
                writer.WriteLine("Content-Type: text/plain");
                writer.WriteLine($"Content-Length: {responseBody.Length}");
                writer.WriteLine();
                writer.Write(responseBody);
            }
            else
            {
                writer.WriteLine("HTTP/1.0 200 OK");
                writer.WriteLine("Content-Type: text/html");
                writer.WriteLine($"Content-Length: {responseBody.Length}");
                writer.WriteLine();
                writer.WriteLine(responseBody);
            }
        }
    }
}