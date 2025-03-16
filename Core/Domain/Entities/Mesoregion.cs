namespace Domain.Entities;

public class Mesoregion
{
    public Mesoregion() { }

    public Mesoregion(long itemId, string itemNome, long stateId)
    {
        Id = itemId;
        Name = itemNome;
        StateId = stateId;
    }

    public long Id { get; set; }
    public string Name { get; set; } = null!;
    public long StateId { get; set; }

    public List<MicroRegion> MicroRegions { get; set; } = new();
    public State State { get; set; } = null!;
}
