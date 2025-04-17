using System.Text;

namespace SEB.Http
{
    public class ServerService
    {
        private Request request = new Request();
        public void ParseRequestLine(StreamReader reader, StreamWriter writer)
        {
            string line = reader.ReadLine() ?? ""; // POST /users HTTP/1.1 

            string[] arr = line.Split(' ');
            request.Method = arr[0];
            request.Path = arr[1];
            request.Version = arr[2];

            Console.WriteLine($"Method: {request.Method}, Path: {request.Path}, Version: {request.Version}");            
        }
        public void ParseHeaders(StreamReader reader, StreamWriter writer)
        {
            while(true)
            { 
                string line = reader.ReadLine() ?? ""; // Content-Type: application/json
                if(line == "") break;

                string[] arr = line.Split(':', 2);
                string headerName = arr[0];
                string headerValue = arr[1].Trim();
                request.Headers[headerName] = headerValue;
                Console.WriteLine($"Header: {headerName} = {headerValue}");

                if(headerName == "Content-Length")
                {
                    request.ContentLength = int.Parse(headerValue);
                }
            }
        }
        public void ParseBody(StreamReader reader, StreamWriter writer)
        {
            StringBuilder requestBody = new StringBuilder();
            if(request.ContentLength > 0)
            {
                char[] chars = new char[1024];
                int bytesReadTotal = 0;
                while(bytesReadTotal < request.ContentLength)
                {
                    var bytesRead = reader.Read(chars, 0, chars.Length);
                    bytesReadTotal += bytesRead;
                    if(bytesRead == 0)
                    {
                        break;  // no more data available
                    }
                    requestBody.Append(chars, 0, bytesRead);
                }
            }
            request.Body = requestBody.ToString();
            Console.WriteLine($"Body: {request.Body}");
        }
    }
}