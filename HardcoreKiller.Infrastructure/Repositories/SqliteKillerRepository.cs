using HardcoreKiller.Domain.Entities;
using HardcoreKiller.Domain.Repositories;
using HardcoreKiller.Shared.Common;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using System.Data;

namespace HardcoreKiller.Infrastructure.Repositories;

public class SqliteKillerRepository : IKillerRepository
{
    private readonly string _connectionString;
    private readonly ILogger<SqliteKillerRepository> _logger;

    public SqliteKillerRepository(string connectionString, ILogger<SqliteKillerRepository> logger)
    {
        _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Result<IEnumerable<Killer>>> GetAllAsync()
    {
        try
        {
            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();

            const string sql = "SELECT id, name, base_cost FROM killers ORDER BY name";
            using var command = new SqliteCommand(sql, connection);
            using var reader = await command.ExecuteReaderAsync();

            var killers = new List<Killer>();
            while (await reader.ReadAsync())
            {
                var killer = MapFromReader(reader);
                killers.Add(killer);
            }

            _logger.LogDebug("Retrieved {Count} killers from database", killers.Count);
            return Result<IEnumerable<Killer>>.Success(killers);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all killers");
            return Result<IEnumerable<Killer>>.Failure($"Failed to retrieve killers: {ex.Message}");
        }
    }

    public async Task<Result<Killer>> GetByIdAsync(string id)
    {
        try
        {
            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();

            const string sql = "SELECT id, name, base_cost FROM killers WHERE id = @id";
            using var command = new SqliteCommand(sql, connection);
            command.Parameters.AddWithValue("@id", id);

            using var reader = await command.ExecuteReaderAsync();
            if (!await reader.ReadAsync())
            {
                return Result<Killer>.Failure($"Killer with ID '{id}' not found");
            }

            var killer = MapFromReader(reader);
            return Result<Killer>.Success(killer);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving killer with ID {KillerId}", id);
            return Result<Killer>.Failure($"Failed to retrieve killer: {ex.Message}");
        }
    }

    public async Task<Result<Killer>> CreateAsync(Killer killer)
    {
        try
        {
            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();

            const string sql = @"
                INSERT INTO killers (id, name, base_cost) 
                VALUES (@id, @name, @baseCost)";

            using var command = new SqliteCommand(sql, connection);
            command.Parameters.AddWithValue("@id", killer.Id);
            command.Parameters.AddWithValue("@name", killer.Name);
            command.Parameters.AddWithValue("@baseCost", killer.BaseCost);

            await command.ExecuteNonQueryAsync();

            _logger.LogInformation("Created killer: {KillerName} (ID: {KillerId})", killer.Name, killer.Id);
            return Result<Killer>.Success(killer);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating killer: {KillerName}", killer.Name);
            return Result<Killer>.Failure($"Failed to create killer: {ex.Message}");
        }
    }

    public async Task<Result<Killer>> UpdateAsync(Killer killer)
    {
        try
        {
            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();

            const string sql = @"
                UPDATE killers 
                SET name = @name, base_cost = @baseCost 
                WHERE id = @id";

            using var command = new SqliteCommand(sql, connection);
            command.Parameters.AddWithValue("@id", killer.Id);
            command.Parameters.AddWithValue("@name", killer.Name);
            command.Parameters.AddWithValue("@baseCost", killer.BaseCost);

            var rowsAffected = await command.ExecuteNonQueryAsync();
            if (rowsAffected == 0)
            {
                return Result<Killer>.Failure($"Killer with ID '{killer.Id}' not found for update");
            }

            _logger.LogInformation("Updated killer: {KillerName} (ID: {KillerId})", killer.Name, killer.Id);
            return Result<Killer>.Success(killer);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating killer: {KillerId}", killer.Id);
            return Result<Killer>.Failure($"Failed to update killer: {ex.Message}");
        }
    }

    public async Task<Result> DeleteAsync(string id)
    {
        try
        {
            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();

            const string sql = "DELETE FROM killers WHERE id = @id";
            using var command = new SqliteCommand(sql, connection);
            command.Parameters.AddWithValue("@id", id);

            var rowsAffected = await command.ExecuteNonQueryAsync();
            if (rowsAffected == 0)
            {
                return Result.Failure($"Killer with ID '{id}' not found for deletion");
            }

            _logger.LogInformation("Deleted killer with ID: {KillerId}", id);
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting killer with ID: {KillerId}", id);
            return Result.Failure($"Failed to delete killer: {ex.Message}");
        }
    }

    public async Task<Result<bool>> ExistsAsync(string id)
    {
        try
        {
            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();

            const string sql = "SELECT 1 FROM killers WHERE id = @id LIMIT 1";
            using var command = new SqliteCommand(sql, connection);
            command.Parameters.AddWithValue("@id", id);

            var result = await command.ExecuteScalarAsync();
            return Result<bool>.Success(result != null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking killer existence: {KillerId}", id);
            return Result<bool>.Failure($"Failed to check killer existence: {ex.Message}");
        }
    }

    public async Task<Result<bool>> ExistsByNameAsync(string name, string? excludeId = null)
    {
        try
        {
            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();

            string sql = "SELECT 1 FROM killers WHERE name = @name";
            if (!string.IsNullOrEmpty(excludeId))
            {
                sql += " AND id != @excludeId";
            }
            sql += " LIMIT 1";

            using var command = new SqliteCommand(sql, connection);
            command.Parameters.AddWithValue("@name", name);
            if (!string.IsNullOrEmpty(excludeId))
            {
                command.Parameters.AddWithValue("@excludeId", excludeId);
            }

            var result = await command.ExecuteScalarAsync();
            return Result<bool>.Success(result != null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking killer name existence: {KillerName}", name);
            return Result<bool>.Failure($"Failed to check killer name existence: {ex.Message}");
        }
    }

    private static Killer MapFromReader(SqliteDataReader reader)
    {
        var id = reader.GetString("id");
        var name = reader.GetString("name");
        var baseCost = reader.GetInt32("base_cost");

        return new Killer(id, name, baseCost);
    }
}