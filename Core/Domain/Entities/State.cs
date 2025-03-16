namespace Domain.Entities;

public class State
{
    public State() { }

    public State(long id, string name, string initials, long regionId)
    {
        Id = id;
        Name = name;
        Initials = initials;
        RegionId = regionId;
    }

    public long Id { get; set; }
    public string Name { get; set; } = null!;
    public string Initials { get; set; } = null!;
    public long RegionId { get; set; }

    public List<Mesoregion> Mesoregions { get; set; } = new();
    public Region Region { get; set; } = null!;
}
