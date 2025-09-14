namespace HardcoreKiller.Shared.Configuration;

public static class DatabaseConstants
{
    public const string DatabaseFileName = "hardcore_killer.db";
    public const int CurrentDatabaseVersion = 1;

    public static class Tables
    {
        public const string DatabaseVersion = "database_version";
        public const string Killers = "killers";
        public const string Ranks = "ranks";
        public const string Challenges = "challenges";
        public const string ChallengeMetadataTypes = "challenge_metadata_types";
        public const string ChallengeMetadataValues = "challenge_metadata_values";
        public const string Matches = "matches";
    }

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
}