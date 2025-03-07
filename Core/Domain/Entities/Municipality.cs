namespace Domain.Entities;

public class Municipality
{
    public int Id { get; private set; }
    public string Name { get; private set; } = null!;
    public int MicroRegionId { get; private set; }

    //Relationships
    public List<Districts> Districts = new List<Districts>();
    public MicroRegion MicroRegion = null!;
}
