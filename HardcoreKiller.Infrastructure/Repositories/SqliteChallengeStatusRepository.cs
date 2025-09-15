using HardcoreKiller.Domain.Entities;
using HardcoreKiller.Domain.Repositories;
using HardcoreKiller.Infrastructure.Database;
using HardcoreKiller.Shared.Common;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using System.Data;

namespace HardcoreKiller.Infrastructure.Repositories;

public class SqliteChallengeStatusRepository : BaseSqliteRepository<ChallengeStatus, ChallengeStatusQueryBuilder>, IChallengeStatusRepository
{
    public SqliteChallengeStatusRepository(
        string connectionString,
        ChallengeStatusQueryBuilder queryBuilder,
        ILogger<SqliteChallengeStatusRepository> logger)
        : base(connectionString, queryBuilder, logger)
    {
    }

    public async Task<Result<IEnumerable<ChallengeStatus>>> GetAllAsync()
    {
        var sql = QueryBuilder.SelectAll();
        return await ExecuteQueryAsync(sql, MapFromReader);
    }

    public async Task<Result<ChallengeStatus>> GetByIdAsync(string id)
    {
        var sql = QueryBuilder.SelectById();
        var parameters = new Dictionary<string, object> { { "id", id } };

        return await ExecuteQuerySingleAsync(sql, MapFromReader, parameters);
    }

    public async Task<Result<ChallengeStatus>> GetByStatusAsync(string status)
    {
        var sql = QueryBuilder.SelectByStatus();
        var parameters = new Dictionary<string, object> { { "status", status } };

        return await ExecuteQuerySingleAsync(sql, MapFromReader, parameters);
    }

    public async Task<Result<ChallengeStatus>> CreateAsync(ChallengeStatus challengeStatus)
    {
        var sql = QueryBuilder.Insert();
        var parameters = new Dictionary<string, object>
        {
            { "id", challengeStatus.Id },
            { "status", challengeStatus.Status }
        };

        var result = await ExecuteNonQueryAsync(sql, parameters);

        if (result.IsSuccess)
        {
            Logger.LogInformation("Created challenge status: {Status} (ID: {Id})",
                challengeStatus.Status, challengeStatus.Id);
            return Result<ChallengeStatus>.Success(challengeStatus);
        }

        return Result<ChallengeStatus>.Failure(result.Error!);
    }

    public async Task<Result<ChallengeStatus>> UpdateAsync(ChallengeStatus challengeStatus)
    {
        var sql = QueryBuilder.Update();
        var parameters = new Dictionary<string, object>
        {
            { "id", challengeStatus.Id },
            { "status", challengeStatus.Status }
        };

        var result = await ExecuteNonQueryAsync(sql, parameters);

        if (result.IsSuccess)
        {
            Logger.LogInformation("Updated challenge status: {Status} (ID: {Id})",
                challengeStatus.Status, challengeStatus.Id);
            return Result<ChallengeStatus>.Success(challengeStatus);
        }

        return Result<ChallengeStatus>.Failure(result.Error!);
    }

    public async Task<Result> DeleteAsync(string id)
    {
        var sql = QueryBuilder.Delete();
        var parameters = new Dictionary<string, object> { { "id", id } };

        var result = await ExecuteNonQueryAsync(sql, parameters);

        if (result.IsSuccess)
        {
            Logger.LogInformation("Deleted challenge status with ID: {Id}", id);
        }

        return result;
    }

    public async Task<Result<bool>> ExistsAsync(string id)
    {
        var sql = QueryBuilder.Exists();
        var parameters = new Dictionary<string, object> { { "id", id } };

        return await ExecuteExistsAsync(sql, parameters);
    }

    public async Task<Result<bool>> ExistsByStatusAsync(string status, string? excludeId = null)
    {
        string sql;
        Dictionary<string, object> parameters;

        if (string.IsNullOrEmpty(excludeId))
        {
            sql = QueryBuilder.ExistsByStatus();
            parameters = new Dictionary<string, object> { { "status", status } };
        }
        else
        {
            sql = QueryBuilder.ExistsByStatusExcluding();
            parameters = new Dictionary<string, object>
            {
                { "status", status },
                { "excludeId", excludeId }
            };
        }

        return await ExecuteExistsAsync(sql, parameters);
    }

    private static ChallengeStatus MapFromReader(SqliteDataReader reader)
    {
        var id = reader.GetString("id");
        var status = reader.GetString("status");

        return new ChallengeStatus(id, status);
    }
}