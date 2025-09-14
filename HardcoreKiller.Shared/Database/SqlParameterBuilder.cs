namespace HardcoreKiller.Shared.Database;

public class SqlParameterBuilder : ISqlParameterBuilder
{
    private readonly Dictionary<string, object> _parameters = [];

    public void AddParameter(string name, object value)
    {
        _parameters[name] = value;
    }

    public IDictionary<string, object> GetParameters() => _parameters;

    public void Clear() => _parameters.Clear();
}