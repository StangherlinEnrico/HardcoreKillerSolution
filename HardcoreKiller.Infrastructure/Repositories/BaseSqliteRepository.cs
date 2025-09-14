using Microsoft.Data.Sqlite;
using HardcoreKiller.Shared.Database;
using HardcoreKiller.Shared.Common;
using Microsoft.Extensions.Logging;

namespace HardcoreKiller.Infrastructure.Repositories;

public abstract class BaseSqliteRepository<T, TQueryBuilder>
    where TQueryBuilder : IQueryBuilder<T>
{
    protected readonly string ConnectionString;
    protected readonly TQueryBuilder QueryBuilder;
    protected readonly ILogger Logger;

    protected BaseSqliteRepository(
        string connectionString,
        TQueryBuilder queryBuilder,
        ILogger logger)
    {
        ConnectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        QueryBuilder = queryBuilder ?? throw new ArgumentNullException(nameof(queryBuilder));
        Logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    // CORRETTO: rimosso <TResult> non necessario
    protected async Task<Result<IEnumerable<T>>> ExecuteQueryAsync(
        string sql,
        Func<SqliteDataReader, T> mapper,
        IDictionary<string, object>? parameters = null)
    {
        try
        {
            using var connection = new SqliteConnection(ConnectionString);
            await connection.OpenAsync();

            using var command = new SqliteCommand(sql, connection);

            if (parameters != null)
            {
                foreach (var param in parameters)
                {
                    command.Parameters.AddWithValue($"@{param.Key}", param.Value);
                }
            }

            using var reader = await command.ExecuteReaderAsync();
            var results = new List<T>();

            while (await reader.ReadAsync())
            {
                results.Add(mapper(reader));
            }

            return Result<IEnumerable<T>>.Success(results);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error executing query: {Sql}", sql);
            return Result<IEnumerable<T>>.Failure($"Query execution failed: {ex.Message}");
        }
    }

    protected async Task<Result<T>> ExecuteQuerySingleAsync(
        string sql,
        Func<SqliteDataReader, T> mapper,
        IDictionary<string, object>? parameters = null)
    {
        try
        {
            using var connection = new SqliteConnection(ConnectionString);
            await connection.OpenAsync();

            using var command = new SqliteCommand(sql, connection);

            if (parameters != null)
            {
                foreach (var param in parameters)
                {
                    command.Parameters.AddWithValue($"@{param.Key}", param.Value);
                }
            }

            using var reader = await command.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
            {
                return Result<T>.Failure("Record not found");
            }

            var result = mapper(reader);
            return Result<T>.Success(result);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error executing single query: {Sql}", sql);
            return Result<T>.Failure($"Query execution failed: {ex.Message}");
        }
    }

    protected async Task<Result> ExecuteNonQueryAsync(
        string sql,
        IDictionary<string, object> parameters)
    {
        try
        {
            using var connection = new SqliteConnection(ConnectionString);
            await connection.OpenAsync();

            using var command = new SqliteCommand(sql, connection);

            foreach (var param in parameters)
            {
                command.Parameters.AddWithValue($"@{param.Key}", param.Value);
            }

            var rowsAffected = await command.ExecuteNonQueryAsync();

            if (rowsAffected == 0)
            {
                return Result.Failure("No rows affected");
            }

            return Result.Success();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error executing non-query: {Sql}", sql);
            return Result.Failure($"Command execution failed: {ex.Message}");
        }
    }

    protected async Task<Result<bool>> ExecuteExistsAsync(
        string sql,
        IDictionary<string, object> parameters)
    {
        try
        {
            using var connection = new SqliteConnection(ConnectionString);
            await connection.OpenAsync();

            using var command = new SqliteCommand(sql, connection);

            foreach (var param in parameters)
            {
                command.Parameters.AddWithValue($"@{param.Key}", param.Value);
            }

            var result = await command.ExecuteScalarAsync();
            return Result<bool>.Success(result != null);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error executing exists query: {Sql}", sql);
            return Result<bool>.Failure($"Exists query failed: {ex.Message}");
        }
    }

    // Metodo helper per eseguire query scalari (es. COUNT)
    protected async Task<Result<TScalar>> ExecuteScalarAsync<TScalar>(
        string sql,
        IDictionary<string, object>? parameters = null)
    {
        try
        {
            using var connection = new SqliteConnection(ConnectionString);
            await connection.OpenAsync();

            using var command = new SqliteCommand(sql, connection);

            if (parameters != null)
            {
                foreach (var param in parameters)
                {
                    command.Parameters.AddWithValue($"@{param.Key}", param.Value);
                }
            }

            var result = await command.ExecuteScalarAsync();

            if (result == null || result == DBNull.Value)
            {
                return Result<TScalar>.Failure("No result returned");
            }

            return Result<TScalar>.Success((TScalar)Convert.ChangeType(result, typeof(TScalar)));
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error executing scalar query: {Sql}", sql);
            return Result<TScalar>.Failure($"Scalar query failed: {ex.Message}");
        }
    }
}