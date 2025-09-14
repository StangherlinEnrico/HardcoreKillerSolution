using HardcoreKiller.Domain.Entities;
using HardcoreKiller.Shared.Common;

namespace HardcoreKiller.Domain.Repositories;

public interface IKillerRepository
{
    Task<Result<IEnumerable<Killer>>> GetAllAsync();
    Task<Result<Killer>> GetByIdAsync(string id);
    Task<Result<Killer>> CreateAsync(Killer killer);
    Task<Result<Killer>> UpdateAsync(Killer killer);
    Task<Result> DeleteAsync(string id);
    Task<Result<bool>> ExistsAsync(string id);
    Task<Result<bool>> ExistsByNameAsync(string name, string? excludeId = null);
}