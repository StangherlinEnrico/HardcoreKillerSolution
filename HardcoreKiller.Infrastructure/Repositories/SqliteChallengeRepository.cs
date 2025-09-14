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
        var sql = QueryBuilder.SelectActive();
        return await ExecuteQueryAsync(sql, MapFromReader);
    }

    public async Task<Result<IEnumerable<Challenge>>> GetCompletedAsync()
    {
        var sql = QueryBuilder.SelectCompleted();
        return await ExecuteQueryAsync(sql, MapFromReader);
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
            { "bank_funds", challenge.BankFunds },
            { "max_bank_funds", challenge.MaxBankFunds },
            { "current_rank_id", challenge.CurrentRankId },
            { "start_date", challenge.StartDate.ToString("yyyy-MM-dd HH:mm:ss") },
            { "end_date", challenge.EndDate?.ToString("yyyy-MM-dd HH:mm:ss") }
        };

        var result = await ExecuteNonQueryAsync(sql, parameters);

        if (result.IsSuccess)
        {
            Logger.LogInformation("Created challenge: ID {ChallengeId}, Funds: {BankFunds}/{MaxBankFunds}",
                challenge.Id, challenge.BankFunds, challenge.MaxBankFunds);
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
            { "bank_funds", challenge.BankFunds },
            { "max_bank_funds", challenge.MaxBankFunds },
            { "current_rank_id", challenge.CurrentRankId },
            { "start_date", challenge.StartDate.ToString("yyyy-MM-dd HH:mm:ss") },
            { "end_date", challenge.EndDate?.ToString("yyyy-MM-dd HH:mm:ss") }
        };

        var result = await ExecuteNonQueryAsync(sql, parameters);

        if (result.IsSuccess)
        {
            Logger.LogInformation("Updated challenge: ID {ChallengeId}, Status: {Status}",
                challenge.Id, challenge.IsActive ? "Active" : "Completed");
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
        var sql = QueryBuilder.ExistsActive();
        return await ExecuteExistsAsync(sql, new Dictionary<string, object>());
    }

    public async Task<Result<Challenge>> GetMostRecentActiveAsync()
    {
        var sql = QueryBuilder.SelectMostRecentActive();
        return await ExecuteQuerySingleAsync(sql, MapFromReader);
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
        var bankFunds = reader.GetInt32("bank_funds");
        var maxBankFunds = reader.GetInt32("max_bank_funds");
        var currentRankId = reader.GetString("current_rank_id");
        var startDate = DateTime.Parse(reader.GetString("start_date"));

        DateTime? endDate = null;
        if (!reader.IsDBNull("end_date"))
        {
            endDate = DateTime.Parse(reader.GetString("end_date"));
        }

        return new Challenge(id, bankFunds, maxBankFunds, currentRankId, startDate, endDate);
    }
}