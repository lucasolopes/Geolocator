namespace Domain.Entities;

public class Mesoregion
{
    public int Id { get; private set; }
    public string Name { get; private set; } = null!;
    public int StateId { get; private set; }

    //Relationships
    public List<MicroRegion> MicroRegions = new List<MicroRegion>();
    public State State = null!;
}
