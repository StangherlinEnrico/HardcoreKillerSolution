namespace HardcoreKiller.Domain.Entities;

public class Challenge
{
    public Challenge(string id, string statusId, int initialBankFunds, int bankFunds, int maxBankFunds, string currentRankId, DateTime startDate, DateTime? endDate = null, string? completionReason = null)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("Challenge ID cannot be null or empty", nameof(id));

        if (string.IsNullOrWhiteSpace(statusId))
            throw new ArgumentException("Status ID cannot be null or empty", nameof(statusId));

        if (initialBankFunds < 0)
            throw new ArgumentException("Initial bank funds cannot be negative", nameof(initialBankFunds));

        if (bankFunds < 0)
            throw new ArgumentException("Bank funds cannot be negative", nameof(bankFunds));

        if (maxBankFunds < 0)
            throw new ArgumentException("Max bank funds cannot be negative", nameof(maxBankFunds));

        if (bankFunds > maxBankFunds)
            throw new ArgumentException("Bank funds cannot exceed max bank funds", nameof(bankFunds));

        if (string.IsNullOrWhiteSpace(currentRankId))
            throw new ArgumentException("Current rank ID cannot be null or empty", nameof(currentRankId));

        if (endDate.HasValue && endDate.Value < startDate)
            throw new ArgumentException("End date cannot be before start date", nameof(endDate));

        Id = id;
        StatusId = statusId;
        InitialBankFunds = initialBankFunds;
        BankFunds = bankFunds;
        MaxBankFunds = maxBankFunds;
        CurrentRankId = currentRankId;
        StartDate = startDate;
        EndDate = endDate;
        CompletionReason = completionReason;
    }

    public string Id { get; private set; }
    public string StatusId { get; private set; }
    public int InitialBankFunds { get; private set; }
    public int BankFunds { get; private set; }
    public int MaxBankFunds { get; private set; }
    public string CurrentRankId { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime? EndDate { get; private set; }
    public string? CompletionReason { get; private set; }

    public TimeSpan Duration => (EndDate ?? DateTime.UtcNow) - StartDate;

    public void UpdateBankFunds(int newBankFunds)
    {
        if (newBankFunds < 0)
            throw new ArgumentException("Bank funds cannot be negative", nameof(newBankFunds));

        if (newBankFunds > MaxBankFunds)
            throw new ArgumentException("Bank funds cannot exceed max bank funds", nameof(newBankFunds));

        BankFunds = newBankFunds;
    }

    public void UpdateMaxBankFunds(int newMaxBankFunds)
    {
        if (newMaxBankFunds < 0)
            throw new ArgumentException("Max bank funds cannot be negative", nameof(newMaxBankFunds));

        if (newMaxBankFunds < BankFunds)
            throw new ArgumentException("Max bank funds cannot be less than current bank funds", nameof(newMaxBankFunds));

        MaxBankFunds = newMaxBankFunds;
    }

    public void UpdateCurrentRank(string newCurrentRankId)
    {
        if (string.IsNullOrWhiteSpace(newCurrentRankId))
            throw new ArgumentException("Current rank ID cannot be null or empty", nameof(newCurrentRankId));

        CurrentRankId = newCurrentRankId;
    }

    public void UpdateStatus(string newStatusId)
    {
        if (string.IsNullOrWhiteSpace(newStatusId))
            throw new ArgumentException("Status ID cannot be null or empty", nameof(newStatusId));

        StatusId = newStatusId;
    }

    public void CompleteChallenge(string statusId, DateTime? completionDate = null, string? reason = null)
    {
        if (string.IsNullOrWhiteSpace(statusId))
            throw new ArgumentException("Status ID cannot be null or empty", nameof(statusId));

        var endDate = completionDate ?? DateTime.UtcNow;
        if (endDate < StartDate)
            throw new ArgumentException("Completion date cannot be before start date", nameof(completionDate));

        StatusId = statusId;
        EndDate = endDate;
        CompletionReason = reason;
    }

    public bool CanAfford(int cost) => BankFunds >= cost;

    public void SpendFunds(int amount)
    {
        if (amount < 0)
            throw new ArgumentException("Amount cannot be negative", nameof(amount));

        if (!CanAfford(amount))
            throw new InvalidOperationException($"Insufficient funds. Available: {BankFunds}, Required: {amount}");

        BankFunds -= amount;
    }

    public void AddFunds(int amount)
    {
        if (amount < 0)
            throw new ArgumentException("Amount cannot be negative", nameof(amount));

        var newTotal = BankFunds + amount;
        if (newTotal > MaxBankFunds)
        {
            BankFunds = MaxBankFunds;
        }
        else
        {
            BankFunds = newTotal;
        }
    }

    public static Challenge Create(string statusId, int initialBankFunds, int maxBankFunds, string currentRankId, DateTime? startDate = null)
    {
        return new Challenge(
            Guid.NewGuid().ToString(),
            statusId,
            initialBankFunds,
            initialBankFunds,
            maxBankFunds,
            currentRankId,
            startDate ?? DateTime.UtcNow
        );
    }

    public double GetFundsPercentage() => MaxBankFunds > 0 ? (double)BankFunds / MaxBankFunds * 100 : 0;
}