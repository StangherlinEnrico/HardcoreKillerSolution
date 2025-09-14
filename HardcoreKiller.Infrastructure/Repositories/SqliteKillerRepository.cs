using HardcoreKiller.Domain.Entities;
using HardcoreKiller.Domain.Repositories;
using HardcoreKiller.Infrastructure.Database;
using HardcoreKiller.Shared.Common;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using System.Data;

namespace HardcoreKiller.Infrastructure.Repositories;

public class SqliteKillerRepository : BaseSqliteRepository<Killer, KillerQueryBuilder>, IKillerRepository
{
    public SqliteKillerRepository(
        string connectionString,
        KillerQueryBuilder queryBuilder,
        ILogger<SqliteKillerRepository> logger)
        : base(connectionString, queryBuilder, logger)
    {
    }

    public async Task<Result<IEnumerable<Killer>>> GetAllAsync()
    {
        var sql = QueryBuilder.SelectAll();
        return await ExecuteQueryAsync(sql, MapFromReader);
    }

    public async Task<Result<Killer>> GetByIdAsync(string id)
    {
        var sql = QueryBuilder.SelectById();
        var parameters = new Dictionary<string, object> { { "id", id } };

        return await ExecuteQuerySingleAsync(sql, MapFromReader, parameters);
    }

    public async Task<Result<Killer>> CreateAsync(Killer killer)
    {
        var sql = QueryBuilder.Insert();
        var parameters = new Dictionary<string, object>
        {
            { "id", killer.Id },
            { "name", killer.Name },
            { "base_cost", killer.BaseCost }
        };

        var result = await ExecuteNonQueryAsync(sql, parameters);

        if (result.IsSuccess)
        {
            Logger.LogInformation("Created killer: {KillerName} (ID: {KillerId})", killer.Name, killer.Id);
            return Result<Killer>.Success(killer);
        }

        return Result<Killer>.Failure(result.Error!);
    }

    public async Task<Result<Killer>> UpdateAsync(Killer killer)
    {
        var sql = QueryBuilder.Update();
        var parameters = new Dictionary<string, object>
        {
            { "id", killer.Id },
            { "name", killer.Name },
            { "base_cost", killer.BaseCost }
        };

        var result = await ExecuteNonQueryAsync(sql, parameters);

        if (result.IsSuccess)
        {
            Logger.LogInformation("Updated killer: {KillerName} (ID: {KillerId})", killer.Name, killer.Id);
            return Result<Killer>.Success(killer);
        }

        return Result<Killer>.Failure(result.Error!);
    }

    public async Task<Result> DeleteAsync(string id)
    {
        var sql = QueryBuilder.Delete();
        var parameters = new Dictionary<string, object> { { "id", id } };

        var result = await ExecuteNonQueryAsync(sql, parameters);

        if (result.IsSuccess)
        {
            Logger.LogInformation("Deleted killer with ID: {KillerId}", id);
        }

        return result;
    }

    public async Task<Result<bool>> ExistsAsync(string id)
    {
        var sql = QueryBuilder.Exists();
        var parameters = new Dictionary<string, object> { { "id", id } };

        return await ExecuteExistsAsync(sql, parameters);
    }

    public async Task<Result<bool>> ExistsByNameAsync(string name, string? excludeId = null)
    {
        string sql;
        Dictionary<string, object> parameters;

        if (string.IsNullOrEmpty(excludeId))
        {
            sql = QueryBuilder.ExistsByField("name");
            parameters = new Dictionary<string, object> { { "name", name } };
        }
        else
        {
            sql = QueryBuilder.ExistsByNameExcluding();
            parameters = new Dictionary<string, object>
            {
                { "name", name },
                { "excludeId", excludeId }
            };
        }

        return await ExecuteExistsAsync(sql, parameters);
    }

    private static Killer MapFromReader(SqliteDataReader reader)
    {
        var id = reader.GetString("id");
        var name = reader.GetString("name");
        var baseCost = reader.GetInt32("base_cost");

        return new Killer(id, name, baseCost);
    }
}