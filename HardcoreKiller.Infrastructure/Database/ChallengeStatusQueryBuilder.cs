using HardcoreKiller.Domain.Entities;

namespace HardcoreKiller.Infrastructure.Database;

public class ChallengeStatusQueryBuilder : BaseQueryBuilder<ChallengeStatus>
{
    protected override string TableName => "challenge_statuses";

    protected override string[] Columns => new[] { "id", "status" };

    protected override string[] PrimaryKeyColumns => new[] { "id" };

    public override string CreateTable()
    {
        return @"
            CREATE TABLE IF NOT EXISTS challenge_statuses (
                id TEXT PRIMARY KEY,
                status TEXT NOT NULL UNIQUE
            );";
    }

    protected override string GetDefaultOrderBy()
    {
        return "status ASC";
    }

    public string ExistsByStatus()
    {
        return $"SELECT 1 FROM {TableName} WHERE status = @status LIMIT 1";
    }

    public string ExistsByStatusExcluding()
    {
        return $"SELECT 1 FROM {TableName} WHERE status = @status AND id != @excludeId LIMIT 1";
    }

    public string SelectByStatus()
    {
        return $"SELECT {string.Join(", ", Columns)} FROM {TableName} WHERE status = @status";
    }
}