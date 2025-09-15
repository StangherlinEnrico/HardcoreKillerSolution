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

        public static readonly (string Name, int Level, int PipRequirement, int OrderIndex)[] DefaultRanks = new[]
{
    ("Ash", 4, 0, 1),
    ("Ash", 3, 1, 2),
    ("Ash", 2, 1, 3),
    ("Ash", 1, 1, 4),
    ("Bronze", 4, 2, 5),
    ("Bronze", 3, 2, 6),
    ("Bronze", 2, 2, 7),
    ("Bronze", 1, 2, 8),
    ("Silver", 4, 3, 9),
    ("Silver", 3, 3, 10),
    ("Silver", 2, 3, 11),
    ("Silver", 1, 3, 12),
    ("Gold", 4, 4, 13),
    ("Gold", 3, 4, 14),
    ("Gold", 2, 4, 15),
    ("Gold", 1, 4, 16),
    ("Iridescent", 4, 5, 17),
    ("Iridescent", 3, 5, 18),
    ("Iridescent", 2, 5, 19),
    ("Iridescent", 1, 5, 20)
};

        public static readonly (string Id, string Status)[] DefaultChallengeStatuses = new[]
{
    ("ongoing", "Ongoing"),
    ("completed", "Completed"),
    ("abandoned", "Abandoned"),
    ("lost", "Lost")
};
    }
}