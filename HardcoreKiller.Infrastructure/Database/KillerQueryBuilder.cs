using HardcoreKiller.Domain.Entities;

namespace HardcoreKiller.Infrastructure.Database;

public class KillerQueryBuilder : BaseQueryBuilder<Killer>
{
    protected override string TableName => "killers";

    protected override string[] Columns => new[] { "id", "name", "base_cost" };

    protected override string[] PrimaryKeyColumns => new[] { "id" };

    public override string CreateTable()
    {
        return @"
            CREATE TABLE IF NOT EXISTS killers (
                id TEXT PRIMARY KEY,
                name TEXT NOT NULL,
                base_cost INTEGER NOT NULL DEFAULT 0
            );";
    }

    // Query specifiche per Killer
    public string ExistsByNameExcluding()
    {
        return $"SELECT 1 FROM {TableName} WHERE name = @name AND id != @excludeId LIMIT 1";
    }

    public string GetByNamePattern()
    {
        return $"SELECT {string.Join(", ", Columns)} FROM {TableName} WHERE name LIKE @namePattern ORDER BY name";
    }

    public string GetByCostRange()
    {
        return $"SELECT {string.Join(", ", Columns)} FROM {TableName} WHERE base_cost BETWEEN @minCost AND @maxCost ORDER BY base_cost, name";
    }
}