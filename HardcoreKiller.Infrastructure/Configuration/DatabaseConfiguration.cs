namespace HardcoreKiller.Infrastructure.Configuration;

public static class DatabaseConfiguration
{
    public static string GetConnectionString(string? customPath = null)
    {
        string dbPath;

        if (!string.IsNullOrEmpty(customPath))
        {
            dbPath = customPath;
        }
        else
        {
            // Modalità portable: database accanto all'eseguibile
            var appDirectory = AppContext.BaseDirectory;
            var dataDirectory = Path.Combine(appDirectory, "Data");
            Directory.CreateDirectory(dataDirectory);
            dbPath = Path.Combine(dataDirectory, "hardcore_killer.db");

            // Alternativa per modalità installata (decommenta se preferisci):
            // var appDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "HardcoreKiller");
            // Directory.CreateDirectory(appDataPath);
            // dbPath = Path.Combine(appDataPath, "hardcore_killer.db");
        }

        return $"Data Source={dbPath}";
    }
}