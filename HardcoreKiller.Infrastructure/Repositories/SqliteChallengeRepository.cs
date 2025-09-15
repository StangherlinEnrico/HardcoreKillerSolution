using HardcoreKiller.Domain.Entities;
using HardcoreKiller.Domain.Repositories;
using HardcoreKiller.Infrastructure.Database;
using HardcoreKiller.Shared.Common;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using System.Data;

namespace HardcoreKiller.Infrastructure.Repositories;

public class SqliteChallengeRepository : BaseSqliteRepository<Challenge, ChallengeQueryBuilder>, IChallengeRepository
{
    public SqliteChallengeRepository(
        string connectionString,
        ChallengeQueryBuilder queryBuilder,
        ILogger<SqliteChallengeRepository> logger)
        : base(connectionString, queryBuilder, logger)
    {
    }

    public async Task<Result<IEnumerable<Challenge>>> GetAllAsync()
    {
        var sql = QueryBuilder.SelectAll();
        return await ExecuteQueryAsync(sql, MapFromReader);
    }

    public async Task<Result<IEnumerable<Challenge>>> GetActiveAsync()
    {
        var sql = QueryBuilder.SelectByStatus();
        var parameters = new Dictionary<string, object> { { "statusId", "active" } };
        return await ExecuteQueryAsync(sql, MapFromReader, parameters);
    }

    public async Task<Result<IEnumerable<Challenge>>> GetCompletedAsync()
    {
        var sql = QueryBuilder.SelectByStatus();
        var parameters = new Dictionary<string, object> { { "statusId", "completed" } };
        return await ExecuteQueryAsync(sql, MapFromReader, parameters);
    }

    public async Task<Result<Challenge>> GetByIdAsync(string id)
    {
        var sql = QueryBuilder.SelectById();
        var parameters = new Dictionary<string, object> { { "id", id } };

        return await ExecuteQuerySingleAsync(sql, MapFromReader, parameters);
    }

    public async Task<Result<Challenge>> CreateAsync(Challenge challenge)
    {
        var sql = QueryBuilder.Insert();
        var parameters = new Dictionary<string, object>
        {
            { "id", challenge.Id },
            { "status_id", challenge.StatusId },
            { "initial_bank_funds", challenge.InitialBankFunds },
            { "bank_funds", challenge.BankFunds },
            { "max_bank_funds", challenge.MaxBankFunds },
            { "current_rank_id", challenge.CurrentRankId },
            { "start_date", challenge.StartDate.ToString("yyyy-MM-dd HH:mm:ss") },
            { "end_date", challenge.EndDate?.ToString("yyyy-MM-dd HH:mm:ss") },
            { "completion_reason", challenge.CompletionReason }
        };

        var result = await ExecuteNonQueryAsync(sql, parameters);

        if (result.IsSuccess)
        {
            Logger.LogInformation("Created challenge: ID {ChallengeId}, Status: {StatusId}",
                challenge.Id, challenge.StatusId);
            return Result<Challenge>.Success(challenge);
        }

        return Result<Challenge>.Failure(result.Error!);
    }

    public async Task<Result<Challenge>> UpdateAsync(Challenge challenge)
    {
        var sql = QueryBuilder.Update();
        var parameters = new Dictionary<string, object>
        {
            { "id", challenge.Id },
            { "status_id", challenge.StatusId },
            { "initial_bank_funds", challenge.InitialBankFunds },
            { "bank_funds", challenge.BankFunds },
            { "max_bank_funds", challenge.MaxBankFunds },
            { "current_rank_id", challenge.CurrentRankId },
            { "start_date", challenge.StartDate.ToString("yyyy-MM-dd HH:mm:ss") },
            { "end_date", challenge.EndDate?.ToString("yyyy-MM-dd HH:mm:ss") },
            { "completion_reason", challenge.CompletionReason }
        };

        var result = await ExecuteNonQueryAsync(sql, parameters);

        if (result.IsSuccess)
        {
            Logger.LogInformation("Updated challenge: ID {ChallengeId}, Status: {StatusId}",
                challenge.Id, challenge.StatusId);
            return Result<Challenge>.Success(challenge);
        }

        return Result<Challenge>.Failure(result.Error!);
    }

    public async Task<Result> DeleteAsync(string id)
    {
        var sql = QueryBuilder.Delete();
        var parameters = new Dictionary<string, object> { { "id", id } };

        var result = await ExecuteNonQueryAsync(sql, parameters);

        if (result.IsSuccess)
        {
            Logger.LogInformation("Deleted challenge with ID: {ChallengeId}", id);
        }

        return result;
    }

    public async Task<Result<bool>> ExistsAsync(string id)
    {
        var sql = QueryBuilder.Exists();
        var parameters = new Dictionary<string, object> { { "id", id } };

        return await ExecuteExistsAsync(sql, parameters);
    }

    public async Task<Result<bool>> HasActiveChallengeAsync()
    {
        var sql = QueryBuilder.SelectByStatus();
        var parameters = new Dictionary<string, object> { { "statusId", "active" } };

        var result = await ExecuteQueryAsync(sql, MapFromReader, parameters);
        return Result<bool>.Success(result.IsSuccess && result.Value!.Any());
    }

    public async Task<Result<Challenge>> GetMostRecentActiveAsync()
    {
        var sql = QueryBuilder.SelectByStatus();
        var parameters = new Dictionary<string, object> { { "statusId", "active" } };

        var result = await ExecuteQueryAsync(sql, MapFromReader, parameters);
        if (result.IsSuccess && result.Value!.Any())
        {
            return Result<Challenge>.Success(result.Value!.First());
        }

        return Result<Challenge>.Failure("No active challenge found");
    }

    public async Task<Result<IEnumerable<Challenge>>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        var sql = QueryBuilder.SelectByDateRange();
        var parameters = new Dictionary<string, object>
        {
            { "startDate", startDate.ToString("yyyy-MM-dd HH:mm:ss") },
            { "endDate", endDate.ToString("yyyy-MM-dd HH:mm:ss") }
        };

        return await ExecuteQueryAsync(sql, MapFromReader, parameters);
    }

    public async Task<Result<IEnumerable<Challenge>>> GetByRankAsync(string rankId)
    {
        var sql = QueryBuilder.SelectByRank();
        var parameters = new Dictionary<string, object> { { "rankId", rankId } };

        return await ExecuteQueryAsync(sql, MapFromReader, parameters);
    }

    private static Challenge MapFromReader(SqliteDataReader reader)
    {
        var id = reader.GetString("id");
        var statusId = reader.GetString("status_id");
        var initialBankFunds = reader.GetInt32("initial_bank_funds");
        var bankFunds = reader.GetInt32("bank_funds");
        var maxBankFunds = reader.GetInt32("max_bank_funds");
        var currentRankId = reader.GetString("current_rank_id");
        var startDate = DateTime.Parse(reader.GetString("start_date"));

        DateTime? endDate = null;
        if (!reader.IsDBNull("end_date"))
        {
            endDate = DateTime.Parse(reader.GetString("end_date"));
        }

        string? completionReason = null;
        if (!reader.IsDBNull("completion_reason"))
        {
            completionReason = reader.GetString("completion_reason");
        }

        return new Challenge(id, statusId, initialBankFunds, bankFunds, maxBankFunds, currentRankId, startDate, endDate, completionReason);
    }
}