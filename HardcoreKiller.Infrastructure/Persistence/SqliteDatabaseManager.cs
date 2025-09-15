using HardcoreKiller.Domain.Repositories;
using HardcoreKiller.Infrastructure.Database;
using HardcoreKiller.Shared.Configuration;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;

namespace HardcoreKiller.Infrastructure.Persistence;

public class SqliteDatabaseManager : IDatabaseManager
{
    private readonly string _connectionString;
    private readonly ILogger<SqliteDatabaseManager> _logger;

    public SqliteDatabaseManager(string connectionString, ILogger<SqliteDatabaseManager> logger)
    {
        _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<bool> DatabaseExistsAsync()
    {
        try
        {
            var connectionStringBuilder = new SqliteConnectionStringBuilder(_connectionString);
            var databasePath = connectionStringBuilder.DataSource;

            var exists = File.Exists(databasePath);
            _logger.LogDebug("Database existence check: {DatabasePath} = {Exists}", databasePath, exists);

            return exists;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking database existence");
            return false;
        }
    }

    public async Task CreateDatabaseAsync()
    {
        try
        {
            var connectionStringBuilder = new SqliteConnectionStringBuilder(_connectionString);
            var databasePath = connectionStringBuilder.DataSource;

            _logger.LogInformation("Creating database at: {DatabasePath}", databasePath);

            // Assicurati che la directory esista
            var directory = Path.GetDirectoryName(databasePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
                _logger.LogDebug("Created database directory: {Directory}", directory);
            }

            // Crea la connessione - SQLite creerà automaticamente il file se non esiste
            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();

            _logger.LogDebug("SQLite connection opened successfully");

            // Crea la tabella per il versioning
            using var command = new SqliteCommand(DatabaseConstants.Queries.CreateVersionTable, connection);
            await command.ExecuteNonQueryAsync();

            _logger.LogDebug("Version table created successfully");

            // Crea tutte le tabelle del dominio
            await CreateDomainTablesAsync(connection);

            // Inserisci i dati di default
            await InsertDefaultDataAsync(connection);

            // Inserisce la versione iniziale
            await InsertInitialVersionAsync(connection);

            _logger.LogInformation("Database created successfully with version {Version} at {Path}",
                DatabaseConstants.CurrentDatabaseVersion, databasePath);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating database");
            throw;
        }
    }

    private async Task CreateDomainTablesAsync(SqliteConnection connection)
    {
        // Usa il query builder per creare la tabella killers
        var killerQueryBuilder = new KillerQueryBuilder();
        using var killersCommand = new SqliteCommand(killerQueryBuilder.CreateTable(), connection);
        await killersCommand.ExecuteNonQueryAsync();

        _logger.LogDebug("Killers table created successfully");

        var rankQueryBuilder = new RankQueryBuilder();
        using var ranksCommand = new SqliteCommand(rankQueryBuilder.CreateTable(), connection);
        await ranksCommand.ExecuteNonQueryAsync();

        _logger.LogDebug("Ranks table created successfully");

        var challengeStatusQueryBuilder = new ChallengeStatusQueryBuilder();
        using var challengeStatusCommand = new SqliteCommand(challengeStatusQueryBuilder.CreateTable(), connection);
        await challengeStatusCommand.ExecuteNonQueryAsync();

        _logger.LogDebug("Challenge statuses table created successfully");

        var challengeQueryBuilder = new ChallengeQueryBuilder();
        using var challengesCommand = new SqliteCommand(challengeQueryBuilder.CreateTable(), connection);
        await challengesCommand.ExecuteNonQueryAsync();

        _logger.LogDebug("Challenges table created successfully");
    }

    private async Task InsertDefaultDataAsync(SqliteConnection connection)
    {
        _logger.LogInformation("Inserting default killer data...");

        var killerQueryBuilder = new KillerQueryBuilder();
        var insertKillerQuery = killerQueryBuilder.Insert();

        foreach (var (name, cost) in DatabaseConstants.DefaultData.DefaultKillers)
        {
            try
            {
                using var command = new SqliteCommand(insertKillerQuery, connection);
                command.Parameters.AddWithValue("@id", Guid.NewGuid().ToString());
                command.Parameters.AddWithValue("@name", name);
                command.Parameters.AddWithValue("@base_cost", cost);

                await command.ExecuteNonQueryAsync();
                _logger.LogDebug("Inserted default killer: {Name} (Cost: {Cost})", name, cost);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to insert default killer: {Name}", name);
                throw;
            }
        }

        _logger.LogInformation("Successfully inserted {Count} default killers",
            DatabaseConstants.DefaultData.DefaultKillers.Length);

        // Inserimento ranks di default
        _logger.LogInformation("Inserting default rank data...");

        var rankQueryBuilder = new RankQueryBuilder();
        var insertRankQuery = rankQueryBuilder.Insert();

        foreach (var (name, level, pipRequirement, orderIndex) in DatabaseConstants.DefaultData.DefaultRanks)
        {
            try
            {
                using var command = new SqliteCommand(insertRankQuery, connection);
                command.Parameters.AddWithValue("@id", Guid.NewGuid().ToString());
                command.Parameters.AddWithValue("@name", name);
                command.Parameters.AddWithValue("@level", level);
                command.Parameters.AddWithValue("@pip_requirement", pipRequirement);
                command.Parameters.AddWithValue("@order_index", orderIndex);

                await command.ExecuteNonQueryAsync();
                _logger.LogDebug("Inserted default rank: {Name} {Level} (Order: {OrderIndex})",
                    name, level, orderIndex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to insert default rank: {Name} {Level}", name, level);
                throw;
            }
        }

        _logger.LogInformation("Successfully inserted {Count} default ranks",
            DatabaseConstants.DefaultData.DefaultRanks.Length);

        _logger.LogInformation("Inserting default challenge status data...");

        var challengeStatusQueryBuilder = new ChallengeStatusQueryBuilder();
        var insertChallengeStatusQuery = challengeStatusQueryBuilder.Insert();

        foreach (var (id, status) in DatabaseConstants.DefaultData.DefaultChallengeStatuses)
        {
            try
            {
                using var command = new SqliteCommand(insertChallengeStatusQuery, connection);
                command.Parameters.AddWithValue("@id", id);
                command.Parameters.AddWithValue("@status", status);

                await command.ExecuteNonQueryAsync();
                _logger.LogDebug("Inserted default challenge status: {Status} (ID: {Id})", status, id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to insert default challenge status: {Status}", status);
                throw;
            }
        }

        _logger.LogInformation("Successfully inserted {Count} default challenge statuses",
            DatabaseConstants.DefaultData.DefaultChallengeStatuses.Length);
    }

    public async Task<int> GetDatabaseVersionAsync()
    {
        try
        {
            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();

            using var command = new SqliteCommand(DatabaseConstants.Queries.GetVersion, connection);
            var result = await command.ExecuteScalarAsync();

            if (result == null)
            {
                _logger.LogWarning("No version found in database, assuming version 0");
                return 0;
            }

            var version = Convert.ToInt32(result);
            _logger.LogDebug("Current database version: {Version}", version);
            return version;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting database version");
            return 0;
        }
    }

    public async Task UpdateDatabaseVersionAsync(int version)
    {
        try
        {
            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();

            using var command = new SqliteCommand(DatabaseConstants.Queries.UpdateVersion, connection);
            command.Parameters.AddWithValue("@version", version);

            var rowsAffected = await command.ExecuteNonQueryAsync();

            if (rowsAffected == 0)
            {
                // Se non esiste ancora un record, lo inserisce
                await InsertInitialVersionAsync(connection, version);
            }

            _logger.LogInformation("Database version updated to {Version}", version);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating database version to {Version}", version);
            throw;
        }
    }

    public async Task InitializeAsync()
    {
        try
        {
            _logger.LogInformation("Starting database initialization...");

            var databaseExists = await DatabaseExistsAsync();

            if (!databaseExists)
            {
                _logger.LogInformation("Database does not exist, creating new database...");
                await CreateDatabaseAsync();
                _logger.LogInformation("New database created successfully");
            }
            else
            {
                _logger.LogInformation("Database exists, checking version compatibility...");
                await CheckAndUpgradeVersionAsync();
            }

            // Verifica finale che tutto sia a posto
            await VerifyDatabaseIntegrityAsync();

            _logger.LogInformation("Database initialization completed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "CRITICAL ERROR during database initialization");
            throw;
        }
    }

    private async Task InsertInitialVersionAsync(SqliteConnection connection, int? version = null)
    {
        var versionToInsert = version ?? DatabaseConstants.CurrentDatabaseVersion;

        using var command = new SqliteCommand(DatabaseConstants.Queries.InsertVersion, connection);
        command.Parameters.AddWithValue("@version", versionToInsert);
        await command.ExecuteNonQueryAsync();

        _logger.LogDebug("Inserted initial database version: {Version}", versionToInsert);
    }

    private async Task CheckAndUpgradeVersionAsync()
    {
        var currentVersion = await GetDatabaseVersionAsync();

        if (currentVersion < DatabaseConstants.CurrentDatabaseVersion)
        {
            _logger.LogInformation("Database version {CurrentVersion} is outdated, upgrading to version {ExpectedVersion}",
                currentVersion, DatabaseConstants.CurrentDatabaseVersion);

            await PerformMigrationsAsync(currentVersion, DatabaseConstants.CurrentDatabaseVersion);
            await UpdateDatabaseVersionAsync(DatabaseConstants.CurrentDatabaseVersion);

            _logger.LogInformation("Database upgraded successfully to version {Version}", DatabaseConstants.CurrentDatabaseVersion);
        }
        else if (currentVersion > DatabaseConstants.CurrentDatabaseVersion)
        {
            _logger.LogWarning("Database version {CurrentVersion} is newer than expected {ExpectedVersion}. This might indicate a newer version of the application was used.",
                currentVersion, DatabaseConstants.CurrentDatabaseVersion);
        }
        else
        {
            _logger.LogInformation("Database version is up to date: {Version}", currentVersion);
        }
    }

    private async Task PerformMigrationsAsync(int fromVersion, int toVersion)
    {
        _logger.LogInformation("Performing database migrations from version {FromVersion} to {ToVersion}", fromVersion, toVersion);

        // Qui aggiungerai la logica delle migrazioni quando sarà necessario
        // Per ora è vuoto perché siamo alla versione 1

        await Task.CompletedTask;
    }

    private async Task VerifyDatabaseIntegrityAsync()
    {
        try
        {
            // Verifica che il database esista
            if (!await DatabaseExistsAsync())
            {
                throw new InvalidOperationException("Database verification failed: database file does not exist");
            }

            // Verifica che la versione sia leggibile
            var version = await GetDatabaseVersionAsync();
            if (version <= 0)
            {
                throw new InvalidOperationException("Database verification failed: invalid version");
            }

            _logger.LogDebug("Database integrity verification passed - Version: {Version}", version);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database integrity verification failed");
            throw;
        }
    }
}