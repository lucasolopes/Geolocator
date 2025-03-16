namespace Domain.Entities;

public class Municipality
{
    public Municipality() { }

    public Municipality(long id, string name, long microRegionId)
    {
        Id = id;
        Name = name;
        MicroRegionId = microRegionId;
    }

    public long Id { get; set; }
    public string Name { get; set; } = null!;
    public long MicroRegionId { get; set; }

    public List<Districts> Districts { get; set; } = new();
    public MicroRegion MicroRegion { get; set; } = null!;
}
