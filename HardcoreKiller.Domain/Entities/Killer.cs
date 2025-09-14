namespace HardcoreKiller.Domain.Entities;

public class Killer
{
    public Killer(string id, string name, int baseCost = 0)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("Killer ID cannot be null or empty", nameof(id));

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Killer name cannot be null or empty", nameof(name));

        if (baseCost < 0)
            throw new ArgumentException("Base cost cannot be negative", nameof(baseCost));

        Id = id;
        Name = name;
        BaseCost = baseCost;
    }

    public string Id { get; private set; }
    public string Name { get; private set; }
    public int BaseCost { get; private set; }

    // Metodi per modificare proprietà (seguendo DDD)
    public void UpdateName(string newName)
    {
        if (string.IsNullOrWhiteSpace(newName))
            throw new ArgumentException("Killer name cannot be null or empty", nameof(newName));

        Name = newName;
    }

    public void UpdateBaseCost(int newBaseCost)
    {
        if (newBaseCost < 0)
            throw new ArgumentException("Base cost cannot be negative", nameof(newBaseCost));

        BaseCost = newBaseCost;
    }

    // Factory method per creazione con nuovo ID
    public static Killer Create(string name, int baseCost = 0)
    {
        return new Killer(Guid.NewGuid().ToString(), name, baseCost);
    }
}