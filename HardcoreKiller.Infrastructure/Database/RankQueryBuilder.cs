using HardcoreKiller.Domain.Entities;

namespace HardcoreKiller.Infrastructure.Database;

public class RankQueryBuilder : BaseQueryBuilder<Rank>
{
    protected override string TableName => "ranks";

    protected override string[] Columns => new[] { "id", "name", "level", "pip_requirement", "order_index" };

    protected override string[] PrimaryKeyColumns => new[] { "id" };

    public override string CreateTable()
    {
        return @"
            CREATE TABLE IF NOT EXISTS ranks (
                id TEXT PRIMARY KEY,
                name TEXT NOT NULL,
                level INTEGER NOT NULL CHECK (level BETWEEN 1 AND 4),
                pip_requirement INTEGER NOT NULL DEFAULT 0,
                order_index INTEGER NOT NULL,
                UNIQUE(name, level),
                UNIQUE(order_index)
            );";
    }

    protected override string GetDefaultOrderBy()
    {
        return "order_index ASC";
    }

    // Query specifiche per Rank
    public string SelectAllOrdered()
    {
        return $"SELECT {string.Join(", ", Columns)} FROM {TableName} ORDER BY order_index ASC";
    }

    public string SelectByOrderIndex()
    {
        return $"SELECT {string.Join(", ", Columns)} FROM {TableName} WHERE order_index = @order_index";
    }

    public string ExistsByNameAndLevel()
    {
        return $"SELECT 1 FROM {TableName} WHERE name = @name AND level = @level LIMIT 1";
    }

    public string ExistsByNameAndLevelExcluding()
    {
        return $"SELECT 1 FROM {TableName} WHERE name = @name AND level = @level AND id != @excludeId LIMIT 1";
    }

    public string ExistsByOrderIndex()
    {
        return $"SELECT 1 FROM {TableName} WHERE order_index = @order_index LIMIT 1";
    }

    public string ExistsByOrderIndexExcluding()
    {
        return $"SELECT 1 FROM {TableName} WHERE order_index = @order_index AND id != @excludeId LIMIT 1";
    }

    public string SelectHighestRank()
    {
        return $"SELECT {string.Join(", ", Columns)} FROM {TableName} ORDER BY order_index DESC LIMIT 1";
    }

    public string SelectLowestRank()
    {
        return $"SELECT {string.Join(", ", Columns)} FROM {TableName} ORDER BY order_index ASC LIMIT 1";
    }

    public string GetByLevelRange()
    {
        return $"SELECT {string.Join(", ", Columns)} FROM {TableName} WHERE level BETWEEN @minLevel AND @maxLevel ORDER BY order_index";
    }

    public string GetByNamePattern()
    {
        return $"SELECT {string.Join(", ", Columns)} FROM {TableName} WHERE name LIKE @namePattern ORDER BY order_index";
    }
}