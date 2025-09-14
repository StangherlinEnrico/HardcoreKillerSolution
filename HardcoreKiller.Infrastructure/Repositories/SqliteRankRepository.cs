using HardcoreKiller.Domain.Entities;
using HardcoreKiller.Domain.Repositories;
using HardcoreKiller.Infrastructure.Database;
using HardcoreKiller.Shared.Common;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using System.Data;

namespace HardcoreKiller.Infrastructure.Repositories;

public class SqliteRankRepository : BaseSqliteRepository<Rank, RankQueryBuilder>, IRankRepository
{
    public SqliteRankRepository(
        string connectionString,
        RankQueryBuilder queryBuilder,
        ILogger<SqliteRankRepository> logger)
        : base(connectionString, queryBuilder, logger)
    {
    }

    public async Task<Result<IEnumerable<Rank>>> GetAllAsync()
    {
        var sql = QueryBuilder.SelectAll();
        return await ExecuteQueryAsync(sql, MapFromReader);
    }

    public async Task<Result<IEnumerable<Rank>>> GetAllOrderedAsync()
    {
        var sql = QueryBuilder.SelectAllOrdered();
        return await ExecuteQueryAsync(sql, MapFromReader);
    }

    public async Task<Result<Rank>> GetByIdAsync(string id)
    {
        var sql = QueryBuilder.SelectById();
        var parameters = new Dictionary<string, object> { { "id", id } };

        return await ExecuteQuerySingleAsync(sql, MapFromReader, parameters);
    }

    public async Task<Result<Rank>> GetByOrderIndexAsync(int orderIndex)
    {
        var sql = QueryBuilder.SelectByOrderIndex();
        var parameters = new Dictionary<string, object> { { "order_index", orderIndex } };

        return await ExecuteQuerySingleAsync(sql, MapFromReader, parameters);
    }

    public async Task<Result<Rank>> CreateAsync(Rank rank)
    {
        var sql = QueryBuilder.Insert();
        var parameters = new Dictionary<string, object>
        {
            { "id", rank.Id },
            { "name", rank.Name },
            { "level", rank.Level },
            { "pip_requirement", rank.PipRequirement },
            { "order_index", rank.OrderIndex }
        };

        var result = await ExecuteNonQueryAsync(sql, parameters);

        if (result.IsSuccess)
        {
            Logger.LogInformation("Created rank: {RankName} {Level} (ID: {RankId})",
                rank.Name, rank.Level, rank.Id);
            return Result<Rank>.Success(rank);
        }

        return Result<Rank>.Failure(result.Error!);
    }

    public async Task<Result<Rank>> UpdateAsync(Rank rank)
    {
        var sql = QueryBuilder.Update();
        var parameters = new Dictionary<string, object>
        {
            { "id", rank.Id },
            { "name", rank.Name },
            { "level", rank.Level },
            { "pip_requirement", rank.PipRequirement },
            { "order_index", rank.OrderIndex }
        };

        var result = await ExecuteNonQueryAsync(sql, parameters);

        if (result.IsSuccess)
        {
            Logger.LogInformation("Updated rank: {RankName} {Level} (ID: {RankId})",
                rank.Name, rank.Level, rank.Id);
            return Result<Rank>.Success(rank);
        }

        return Result<Rank>.Failure(result.Error!);
    }

    public async Task<Result> DeleteAsync(string id)
    {
        var sql = QueryBuilder.Delete();
        var parameters = new Dictionary<string, object> { { "id", id } };

        var result = await ExecuteNonQueryAsync(sql, parameters);

        if (result.IsSuccess)
        {
            Logger.LogInformation("Deleted rank with ID: {RankId}", id);
        }

        return result;
    }

    public async Task<Result<bool>> ExistsAsync(string id)
    {
        var sql = QueryBuilder.Exists();
        var parameters = new Dictionary<string, object> { { "id", id } };

        return await ExecuteExistsAsync(sql, parameters);
    }

    public async Task<Result<bool>> ExistsByNameAndLevelAsync(string name, int level, string? excludeId = null)
    {
        string sql;
        Dictionary<string, object> parameters;

        if (string.IsNullOrEmpty(excludeId))
        {
            sql = QueryBuilder.ExistsByNameAndLevel();
            parameters = new Dictionary<string, object>
            {
                { "name", name },
                { "level", level }
            };
        }
        else
        {
            sql = QueryBuilder.ExistsByNameAndLevelExcluding();
            parameters = new Dictionary<string, object>
            {
                { "name", name },
                { "level", level },
                { "excludeId", excludeId }
            };
        }

        return await ExecuteExistsAsync(sql, parameters);
    }

    public async Task<Result<bool>> ExistsByOrderIndexAsync(int orderIndex, string? excludeId = null)
    {
        string sql;
        Dictionary<string, object> parameters;

        if (string.IsNullOrEmpty(excludeId))
        {
            sql = QueryBuilder.ExistsByOrderIndex();
            parameters = new Dictionary<string, object> { { "order_index", orderIndex } };
        }
        else
        {
            sql = QueryBuilder.ExistsByOrderIndexExcluding();
            parameters = new Dictionary<string, object>
            {
                { "order_index", orderIndex },
                { "excludeId", excludeId }
            };
        }

        return await ExecuteExistsAsync(sql, parameters);
    }

    public async Task<Result<Rank>> GetHighestRankAsync()
    {
        var sql = QueryBuilder.SelectHighestRank();
        return await ExecuteQuerySingleAsync(sql, MapFromReader);
    }

    public async Task<Result<Rank>> GetLowestRankAsync()
    {
        var sql = QueryBuilder.SelectLowestRank();
        return await ExecuteQuerySingleAsync(sql, MapFromReader);
    }

    private static Rank MapFromReader(SqliteDataReader reader)
    {
        var id = reader.GetString("id");
        var name = reader.GetString("name");
        var level = reader.GetInt32("level");
        var pipRequirement = reader.GetInt32("pip_requirement");
        var orderIndex = reader.GetInt32("order_index");

        return new Rank(id, name, level, pipRequirement, orderIndex);
    }
}