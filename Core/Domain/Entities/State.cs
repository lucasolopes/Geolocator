namespace Domain.Entities;

public class State
{
    // Construtor sem parâmetros para desserialização
    public State() { }

    // Construtor para uso normal
    public State(int id, string name, string initials, int regionId)
    {
        Id = id;
        Name = name;
        Initials = initials;
        RegionId = regionId;
    }

    public int Id { get; private set; }
    public string Name { get; private set; } = null!;
    public string Initials { get; private set; } = null!;
    public int RegionId { get; private set; }

    //Relationships
    public List<Mesoregion> Mesoregions { get; private set; } = new List<Mesoregion>();
    public Region Region { get; private set; } = null!;
}
