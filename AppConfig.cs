using System.Text.Json;

public static class AppConfig
{
    private static readonly string connectionString;

    static AppConfig()
    {
        string json = File.ReadAllText("config.json");
        var config = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
        connectionString = config?["ConnectionString"] ?? throw new Exception("ConnectionString not found in config.json");
    }

    public static string ConnectionString => connectionString;
}
