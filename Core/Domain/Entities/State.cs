namespace Domain.Entities;

public class State
{
    public int Id { get; private set; }
    public string Name { get; private set; } = null!;
    public string Initials { get; private set; } = null!;
    public int RegionId { get; private set; }

    //Relationships
    public List<Mesoregion> Mesoregions = new List<Mesoregion>();
    public Region Region = null!;
}
