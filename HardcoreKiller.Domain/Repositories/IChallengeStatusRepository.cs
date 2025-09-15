using HardcoreKiller.Domain.Entities;
using HardcoreKiller.Shared.Common;

namespace HardcoreKiller.Domain.Repositories;

public interface IChallengeStatusRepository
{
    Task<Result<IEnumerable<ChallengeStatus>>> GetAllAsync();
    Task<Result<ChallengeStatus>> GetByIdAsync(string id);
    Task<Result<ChallengeStatus>> GetByStatusAsync(string status);
    Task<Result<ChallengeStatus>> CreateAsync(ChallengeStatus challengeStatus);
    Task<Result<ChallengeStatus>> UpdateAsync(ChallengeStatus challengeStatus);
    Task<Result> DeleteAsync(string id);
    Task<Result<bool>> ExistsAsync(string id);
    Task<Result<bool>> ExistsByStatusAsync(string status, string? excludeId = null);
}