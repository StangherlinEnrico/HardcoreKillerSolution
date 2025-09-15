using HardcoreKiller.Domain.Entities;

namespace HardcoreKiller.Infrastructure.Database;

public class ChallengeQueryBuilder : BaseQueryBuilder<Challenge>
{
    protected override string TableName => "challenges";

    protected override string[] Columns => new[] { "id", "status_id", "initial_bank_funds", "bank_funds", "max_bank_funds", "current_rank_id", "start_date", "end_date", "completion_reason" };

    protected override string[] PrimaryKeyColumns => new[] { "id" };

    public override string CreateTable()
    {
        return @"
            CREATE TABLE IF NOT EXISTS challenges (
                id TEXT PRIMARY KEY,
                status_id TEXT NOT NULL,
                initial_bank_funds INTEGER NOT NULL CHECK (initial_bank_funds >= 0),
                bank_funds INTEGER NOT NULL CHECK (bank_funds >= 0),
                max_bank_funds INTEGER NOT NULL CHECK (max_bank_funds >= 0),
                current_rank_id TEXT NOT NULL,
                start_date TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
                end_date TEXT,
                completion_reason TEXT,
                CHECK (bank_funds <= max_bank_funds),
                FOREIGN KEY (status_id) REFERENCES challenge_statuses (id),
                FOREIGN KEY (current_rank_id) REFERENCES ranks (id)
            );";
    }

    protected override string GetDefaultOrderBy()
    {
        return "start_date DESC";
    }

    public string SelectByStatus()
    {
        return $"SELECT {string.Join(", ", Columns)} FROM {TableName} WHERE status_id = @statusId ORDER BY start_date DESC";
    }

    public string SelectByDateRange()
    {
        return $"SELECT {string.Join(", ", Columns)} FROM {TableName} WHERE start_date >= @startDate AND start_date <= @endDate ORDER BY start_date DESC";
    }

    public string SelectByRank()
    {
        return $"SELECT {string.Join(", ", Columns)} FROM {TableName} WHERE current_rank_id = @rankId ORDER BY start_date DESC";
    }
}