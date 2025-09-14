using HardcoreKiller.Domain.Entities;

namespace HardcoreKiller.Infrastructure.Database;

public class ChallengeQueryBuilder : BaseQueryBuilder<Challenge>
{
    protected override string TableName => "challenges";

    protected override string[] Columns => new[] { "id", "bank_funds", "max_bank_funds", "current_rank_id", "start_date", "end_date" };

    protected override string[] PrimaryKeyColumns => new[] { "id" };

    public override string CreateTable()
    {
        return @"
            CREATE TABLE IF NOT EXISTS challenges (
                id TEXT PRIMARY KEY,
                bank_funds INTEGER NOT NULL CHECK (bank_funds >= 0),
                max_bank_funds INTEGER NOT NULL CHECK (max_bank_funds >= 0),
                current_rank_id TEXT NOT NULL,
                start_date TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
                end_date TEXT,
                CHECK (bank_funds <= max_bank_funds),
                FOREIGN KEY (current_rank_id) REFERENCES ranks (id)
            );";
    }

    protected override string GetDefaultOrderBy()
    {
        return "start_date DESC";
    }

    // Query specifiche per Challenge
    public string SelectActive()
    {
        return $"SELECT {string.Join(", ", Columns)} FROM {TableName} WHERE end_date IS NULL ORDER BY start_date DESC";
    }

    public string SelectCompleted()
    {
        return $"SELECT {string.Join(", ", Columns)} FROM {TableName} WHERE end_date IS NOT NULL ORDER BY end_date DESC";
    }

    public string SelectMostRecentActive()
    {
        return $"SELECT {string.Join(", ", Columns)} FROM {TableName} WHERE end_date IS NULL ORDER BY start_date DESC LIMIT 1";
    }

    public string ExistsActive()
    {
        return $"SELECT 1 FROM {TableName} WHERE end_date IS NULL LIMIT 1";
    }

    public string SelectByDateRange()
    {
        return $"SELECT {string.Join(", ", Columns)} FROM {TableName} WHERE start_date >= @startDate AND start_date <= @endDate ORDER BY start_date DESC";
    }

    public string SelectByRank()
    {
        return $"SELECT {string.Join(", ", Columns)} FROM {TableName} WHERE current_rank_id = @rankId ORDER BY start_date DESC";
    }

    public string SelectByDuration()
    {
        return $"SELECT {string.Join(", ", Columns)} FROM {TableName} WHERE end_date IS NOT NULL AND julianday(end_date) - julianday(start_date) BETWEEN @minDays AND @maxDays ORDER BY start_date DESC";
    }

    public string SelectWithBankFundsRange()
    {
        return $"SELECT {string.Join(", ", Columns)} FROM {TableName} WHERE bank_funds BETWEEN @minFunds AND @maxFunds ORDER BY bank_funds DESC";
    }
}