using HardcoreKiller.Domain.Entities;
using HardcoreKiller.Shared.Common;

namespace HardcoreKiller.Domain.Repositories;

public interface IRankRepository
{
    Task<Result<IEnumerable<Rank>>> GetAllAsync();
    Task<Result<IEnumerable<Rank>>> GetAllOrderedAsync();
    Task<Result<Rank>> GetByIdAsync(string id);
    Task<Result<Rank>> GetByOrderIndexAsync(int orderIndex);
    Task<Result<Rank>> CreateAsync(Rank rank);
    Task<Result<Rank>> UpdateAsync(Rank rank);
    Task<Result> DeleteAsync(string id);
    Task<Result<bool>> ExistsAsync(string id);
    Task<Result<bool>> ExistsByNameAndLevelAsync(string name, int level, string? excludeId = null);
    Task<Result<bool>> ExistsByOrderIndexAsync(int orderIndex, string? excludeId = null);
    Task<Result<Rank>> GetHighestRankAsync();
    Task<Result<Rank>> GetLowestRankAsync();
}