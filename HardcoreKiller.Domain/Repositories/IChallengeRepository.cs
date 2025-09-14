using HardcoreKiller.Domain.Entities;
using HardcoreKiller.Shared.Common;

namespace HardcoreKiller.Domain.Repositories;

public interface IChallengeRepository
{
    Task<Result<IEnumerable<Challenge>>> GetAllAsync();
    Task<Result<IEnumerable<Challenge>>> GetActiveAsync();
    Task<Result<IEnumerable<Challenge>>> GetCompletedAsync();
    Task<Result<Challenge>> GetByIdAsync(string id);
    Task<Result<Challenge>> CreateAsync(Challenge challenge);
    Task<Result<Challenge>> UpdateAsync(Challenge challenge);
    Task<Result> DeleteAsync(string id);
    Task<Result<bool>> ExistsAsync(string id);
    Task<Result<bool>> HasActiveChallengeAsync();
    Task<Result<Challenge>> GetMostRecentActiveAsync();
    Task<Result<IEnumerable<Challenge>>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<Result<IEnumerable<Challenge>>> GetByRankAsync(string rankId);
}