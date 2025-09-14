namespace HardcoreKiller.Domain.Repositories;

public interface IDatabaseManager
{
    Task<bool> DatabaseExistsAsync();
    Task CreateDatabaseAsync();
    Task<int> GetDatabaseVersionAsync();
    Task UpdateDatabaseVersionAsync(int version);
    Task InitializeAsync();
}