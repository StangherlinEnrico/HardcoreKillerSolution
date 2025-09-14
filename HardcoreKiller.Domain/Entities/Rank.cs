namespace HardcoreKiller.Domain.Entities;

public class Rank
{
    public Rank(string id, string name, int level, int pipRequirement, int orderIndex)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("Rank ID cannot be null or empty", nameof(id));

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Rank name cannot be null or empty", nameof(name));

        if (level < 1 || level > 4)
            throw new ArgumentException("Rank level must be between 1 and 4", nameof(level));

        if (pipRequirement < 0)
            throw new ArgumentException("Pip requirement cannot be negative", nameof(pipRequirement));

        if (orderIndex < 0)
            throw new ArgumentException("Order index cannot be negative", nameof(orderIndex));

        Id = id;
        Name = name;
        Level = level;
        PipRequirement = pipRequirement;
        OrderIndex = orderIndex;
    }

    public string Id { get; private set; }
    public string Name { get; private set; }
    public int Level { get; private set; }
    public int PipRequirement { get; private set; }
    public int OrderIndex { get; private set; }

    // Metodi per modificare proprietà (seguendo DDD)
    public void UpdateName(string newName)
    {
        if (string.IsNullOrWhiteSpace(newName))
            throw new ArgumentException("Rank name cannot be null or empty", nameof(newName));

        Name = newName;
    }

    public void UpdateLevel(int newLevel)
    {
        if (newLevel < 1 || newLevel > 4)
            throw new ArgumentException("Rank level must be between 1 and 4", nameof(newLevel));

        Level = newLevel;
    }

    public void UpdatePipRequirement(int newPipRequirement)
    {
        if (newPipRequirement < 0)
            throw new ArgumentException("Pip requirement cannot be negative", nameof(newPipRequirement));

        PipRequirement = newPipRequirement;
    }

    public void UpdateOrderIndex(int newOrderIndex)
    {
        if (newOrderIndex < 0)
            throw new ArgumentException("Order index cannot be negative", nameof(newOrderIndex));

        OrderIndex = newOrderIndex;
    }

    // Factory method per creazione con nuovo ID
    public static Rank Create(string name, int level, int pipRequirement, int orderIndex)
    {
        return new Rank(Guid.NewGuid().ToString(), name, level, pipRequirement, orderIndex);
    }

    // Helper per il display del rank completo
    public string GetDisplayName() => $"{Name} {Level}";

    // Helper per confronto ordinamento
    public bool IsHigherThan(Rank other) => OrderIndex > other.OrderIndex;
}