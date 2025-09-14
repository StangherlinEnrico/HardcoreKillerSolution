using HardcoreKiller.Shared.Database;
using System.Reflection;
using System.Text;

namespace HardcoreKiller.Infrastructure.Database;

public abstract class BaseQueryBuilder<T> : IQueryBuilder<T>
{
    protected abstract string TableName { get; }
    protected abstract string[] Columns { get; }
    protected abstract string[] PrimaryKeyColumns { get; }

    public virtual string SelectAll()
    {
        return $"SELECT {string.Join(", ", Columns)} FROM {TableName} ORDER BY {GetDefaultOrderBy()}";
    }

    public virtual string SelectById()
    {
        var whereClause = string.Join(" AND ", PrimaryKeyColumns.Select(col => $"{col} = @{col}"));
        return $"SELECT {string.Join(", ", Columns)} FROM {TableName} WHERE {whereClause}";
    }

    public virtual string Insert()
    {
        var columnNames = string.Join(", ", Columns);
        var parameterNames = string.Join(", ", Columns.Select(col => $"@{col}"));
        return $"INSERT INTO {TableName} ({columnNames}) VALUES ({parameterNames})";
    }

    public virtual string Update()
    {
        var setClause = string.Join(", ",
            Columns.Where(col => !PrimaryKeyColumns.Contains(col))
                   .Select(col => $"{col} = @{col}"));

        var whereClause = string.Join(" AND ", PrimaryKeyColumns.Select(col => $"{col} = @{col}"));

        return $"UPDATE {TableName} SET {setClause} WHERE {whereClause}";
    }

    public virtual string Delete()
    {
        var whereClause = string.Join(" AND ", PrimaryKeyColumns.Select(col => $"{col} = @{col}"));
        return $"DELETE FROM {TableName} WHERE {whereClause}";
    }

    public virtual string Exists()
    {
        var whereClause = string.Join(" AND ", PrimaryKeyColumns.Select(col => $"{col} = @{col}"));
        return $"SELECT 1 FROM {TableName} WHERE {whereClause} LIMIT 1";
    }

    public virtual string ExistsByField(string fieldName)
    {
        return $"SELECT 1 FROM {TableName} WHERE {fieldName} = @{fieldName} LIMIT 1";
    }

    public virtual string Count()
    {
        return $"SELECT COUNT(*) FROM {TableName}";
    }

    public abstract string CreateTable();

    protected virtual string GetDefaultOrderBy()
    {
        // Default al primo campo che contiene "name", altrimenti prima colonna
        var nameColumn = Columns.FirstOrDefault(c => c.Contains("name"));
        return nameColumn ?? Columns[0];
    }

    // Helper per query più complesse
    protected string BuildWhereClause(params string[] conditions)
    {
        return conditions.Length > 0 ? $" WHERE {string.Join(" AND ", conditions)}" : "";
    }

    protected string BuildOrderClause(string column, bool ascending = true)
    {
        return $" ORDER BY {column} {(ascending ? "ASC" : "DESC")}";
    }
}