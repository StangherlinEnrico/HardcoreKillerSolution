namespace HardcoreKiller.Domain.Entities;

public class ChallengeStatus
{
    public ChallengeStatus(string id, string status)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("Challenge status ID cannot be null or empty", nameof(id));

        if (string.IsNullOrWhiteSpace(status))
            throw new ArgumentException("Status cannot be null or empty", nameof(status));

        Id = id;
        Status = status;
    }

    public string Id { get; private set; }
    public string Status { get; private set; }

    public void UpdateStatus(string newStatus)
    {
        if (string.IsNullOrWhiteSpace(newStatus))
            throw new ArgumentException("Status cannot be null or empty", nameof(newStatus));

        Status = newStatus;
    }

    public static ChallengeStatus Create(string status)
    {
        return new ChallengeStatus(Guid.NewGuid().ToString(), status);
    }
}