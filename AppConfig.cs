using System.Text.Json;

public static class AppConfig
{
    private static readonly string connectionString;
    private static readonly int serverPort;

    static AppConfig()
    {
        string json = File.ReadAllText("config.json");
        var config = JsonSerializer.Deserialize<Dictionary<string, string>>(json);

        connectionString = config?["ConnectionString"] ?? throw new Exception("ConnectionString not found in config.json");
        serverPort = int.Parse(config?["ServerPort"] ?? "10001");
    }

    public static string ConnectionString => connectionString;
    public static int ServerPort => serverPort;
    
}
