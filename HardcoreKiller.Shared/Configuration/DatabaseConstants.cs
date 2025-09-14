namespace HardcoreKiller.Shared.Configuration;

public static class DatabaseConstants
{
    public const string DatabaseFileName = "hardcore_killer.db";
    public const int CurrentDatabaseVersion = 1;

    public static class Queries
    {
        public const string CreateVersionTable = @"
            CREATE TABLE IF NOT EXISTS database_version (
                id INTEGER PRIMARY KEY CHECK (id = 1),
                version INTEGER NOT NULL,
                created_at TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
                updated_at TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP
            );";

        public const string GetVersion = "SELECT version FROM database_version WHERE id = 1;";
        public const string InsertVersion = "INSERT INTO database_version (id, version) VALUES (1, @version);";
        public const string UpdateVersion = "UPDATE database_version SET version = @version, updated_at = CURRENT_TIMESTAMP WHERE id = 1;";
    }

    public static class DefaultData
    {
        public static readonly (string Name, int Cost)[] DefaultKillers = new[]
        {
        ("Artist", 12),
        ("Blight", 20),
        ("Bubba", 6),
        ("Cenobite", 18),
        ("Chucky", 12),
        ("Clown", 8),
        ("Deathslinger", 12),
        ("Demogorgon", 10),
        ("Doctor", 8),
        ("Dracula", 14),
        ("Dredge", 6),
        ("Executioner", 12),
        ("Freddy", 8),
        ("Ghost Face", 6),
        ("Ghoul", 18),
        ("Hag", 6),
        ("Hillbilly", 18),
        ("Houndmaster", 14),
        ("Huntress", 14),
        ("Knight", 8),
        ("Legion", 6),
        ("Mike Myers", 6),
        ("Nemesis", 10),
        ("Nurse", 20),
        ("Oni", 12),
        ("Onryo", 6),
        ("Pig", 6),
        ("Plague", 12),
        ("Singularity", 14),
        ("Skull Merchant", 4),
        ("Spirit", 14),
        ("Trapper", 4),
        ("Trickster", 8),
        ("Twins", 12),
        ("Unknown", 12),
        ("Vecna", 8),
        ("Wesker", 12),
        ("Wraith", 10),
        ("Xenomorph", 10)
    };
    }
}