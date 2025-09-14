namespace HardcoreKiller.Shared.Database;

public interface IQueryBuilder<T>
{
    string SelectAll();
    string SelectById();
    string Insert();
    string Update();
    string Delete();
    string Exists();
    string ExistsByField(string fieldName);
    string Count();
    string CreateTable();
}

public interface ISqlParameterBuilder
{
    void AddParameter(string name, object value);
    IDictionary<string, object> GetParameters();
    void Clear();
}