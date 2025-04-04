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
                //Console.WriteLine($"Header: {headerName}={headerValue}");
                if(headerName == "Content-Length")
                {
                    content_length = int.Parse(headerValue);
                    Console.WriteLine($"Header: Content-Length={headerValue}");
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
                Console.WriteLine($"Body: {requestBody.ToString()}");
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
                        throw new JsonException("Invalid JSON format");
                    }
                    if(string.IsNullOrEmpty(user.Username))
                        throw new InvalidOperationException("Username is empty.");

                    if(userRepository.ExistsByUsername(user.Username))
                        throw new InvalidOperationException("Username already exists.");

                    userRepository.Add(user);
                    responseBody = $"User {user.Username} registered successfully";
                    writer.WriteLine("HTTP/1.1 201 Created");
                    writer.WriteLine("Content-Type: text/plain");
                    writer.WriteLine($"Content-Length: {responseBody.Length}");
                    writer.WriteLine();
                    writer.Write(responseBody);
                }
                catch(JsonException ex)
                {
                    responseBody = ex.Message;
                    writer.WriteLine("HTTP/1.1 400 Bad Request");
                    writer.WriteLine("Content-Type: text/plain");
                    writer.WriteLine($"Content-Length: {responseBody.Length}");
                    writer.WriteLine();
                    writer.WriteLine(responseBody);
                }
                catch(InvalidOperationException ex)
                {
                    responseBody = ex.Message;
                    writer.WriteLine("HTTP/1.1 409 Conflict");
                    writer.WriteLine("Content-Type: text/plain");
                    writer.WriteLine($"Content-Length: {responseBody.Length}");
                    writer.WriteLine();
                    writer.WriteLine(responseBody);
                }
                catch(Exception ex)
                {
                    responseBody = $"Unexpected error: {ex.Message}";
                    writer.WriteLine("HTTP/1.1 500 Internal Server Error");
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
                writer.WriteLine("HTTP/1.1 200 OK");
                writer.WriteLine("Content-Type: application/json");
                writer.WriteLine($"Content-Length: {responseBody.Length}");
                writer.WriteLine();
                writer.Write(responseBody);
            }
            else if(method == "GET" && path.StartsWith("/users/"))
            {
                try
                {
                    string name = path.Substring("/users/".Length); // Extracts the part after "/users/" (e.g., "/users/123" → "123")
                    User? user = userRepository.GetByUsername(name);
                    
                    if(user == null)
                    {
                        responseBody = "User not found";
                        writer.WriteLine("HTTP/1.1 404 Not Found");
                        writer.WriteLine("Content-Type: text/plain");
                    }
                    else
                    {
                        responseBody = JsonSerializer.Serialize(user);
                        writer.WriteLine("HTTP/1.1 200 OK");
                        writer.WriteLine("Content-Type: application/json");
                    }


                    writer.WriteLine($"Content-Length: {responseBody.Length}");
                    writer.WriteLine();
                    writer.WriteLine(responseBody);

                }
                catch(Exception ex)
                {
                    responseBody = $"Unexpected error: {ex.Message}";
                    writer.WriteLine("HTTP/1.1 500 Internal Server Error");
                    writer.WriteLine("Content-Type: text/plain");
                    writer.WriteLine($"Content-Length: {responseBody.Length}");
                    writer.WriteLine();
                    writer.WriteLine(responseBody);
                }
            }
            else if(method == "DELETE" && path == "/users")
            {
                userRepository.DeleteAll();
                responseBody = "All users deleted successfully";
                writer.WriteLine("HTTP/1.1 200 OK");
                writer.WriteLine("Content-Type: text/plain");
                writer.WriteLine($"Content-Length: {responseBody.Length}");
                writer.WriteLine();
                writer.Write(responseBody);             
            }
            else if(method == "DELETE" && path.StartsWith("/users/"))
            {
                try
                {
                    string idStr = path.Substring("/users/".Length);
                    int id = int.Parse(idStr);
                    User user = new User { Id = id };
                    userRepository.Delete(user);
                    responseBody = "User deleted";
                    writer.WriteLine("HTTP/1.1 200 OK");
                    writer.WriteLine("Content-Type: text/plain");
                    writer.WriteLine($"Content-Length: {responseBody.Length}");
                    writer.WriteLine();
                    writer.WriteLine(responseBody);
                }
                catch(FormatException)
                {
                    responseBody = "Invalid ID format";
                    writer.WriteLine("HTTP/1.1 400 Bad Request");
                    writer.WriteLine("Content-Type: text/plain");
                    writer.WriteLine($"Content-Length: {responseBody.Length}");
                    writer.WriteLine();
                    writer.WriteLine(responseBody);
                }
                catch(Exception ex)
                {
                    responseBody = $"Unexpected error: {ex.Message}";
                    writer.WriteLine("HTTP/1.1 500 Internal Server Error");
                    writer.WriteLine("Content-Type: text/plain");
                    writer.WriteLine($"Content-Length: {responseBody.Length}");
                    writer.WriteLine();
                    writer.WriteLine(responseBody);
                }
            }
            else if(method == "POST" && path == "/sessions")
            {
                try
                {
                    string body = requestBody.ToString();
                    User? user = JsonSerializer.Deserialize<User>(body);
                    if(user == null)
                    {
                        throw new JsonException("Invalid JSON format");
                    }

                    if(string.IsNullOrEmpty(user.Username))
                        throw new InvalidOperationException("Username is empty.");

                    //if(!userRepository.ExistsByUsername(user.Username))
                        //throw new InvalidOperationException("Username does not exist.");

                    //if(!userRepository.CheckCredentials(user.Username, user.Password))
                        //throw new InvalidOperationException("Invalid username or password.");

                    if(string.IsNullOrEmpty(user.Password) || !userRepository.CheckCredentials(user.Username, user.Password))
                    throw new InvalidOperationException("Invalid username or password.");

                    user.Token = $"{user.Username}-sebToken";
                    responseBody = $"User {user.Username} token: {user.Token}";
                    writer.WriteLine("HTTP/1.1 201 Created");
                    writer.WriteLine("Content-Type: text/plain");
                    writer.WriteLine($"Content-Length: {responseBody.Length}");
                    writer.WriteLine();
                    writer.Write(responseBody);
                }
                catch(JsonException ex)
                {
                    responseBody = ex.Message;
                    writer.WriteLine("HTTP/1.1 400 Bad Request");
                    writer.WriteLine("Content-Type: text/plain");
                    writer.WriteLine($"Content-Length: {responseBody.Length}");
                    writer.WriteLine();
                    writer.WriteLine(responseBody);
                }
                catch(InvalidOperationException ex)
                {
                    responseBody = ex.Message;
                    writer.WriteLine("HTTP/1.1 409 Conflict");
                    writer.WriteLine("Content-Type: text/plain");
                    writer.WriteLine($"Content-Length: {responseBody.Length}");
                    writer.WriteLine();
                    writer.WriteLine(responseBody);
                }
                catch(Exception ex)
                {
                    responseBody = $"Unexpected error: {ex.Message}";
                    writer.WriteLine("HTTP/1.1 500 Internal Server Error");
                    writer.WriteLine("Content-Type: text/plain");
                    writer.WriteLine($"Content-Length: {responseBody.Length}");
                    writer.WriteLine();
                    writer.WriteLine(responseBody);
                }   
            }
            else
            {
                responseBody = "Endpoint not found";
                writer.WriteLine("HTTP/1.1 404 Not Found");
                writer.WriteLine("Content-Type: text/plain");
                writer.WriteLine($"Content-Length: {responseBody.Length}");
                writer.WriteLine();
                writer.WriteLine(responseBody);
            }
        }
    }
}