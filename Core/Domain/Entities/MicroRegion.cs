using System.Runtime.CompilerServices;

namespace Domain.Entities;

public class MicroRegion
{
    public long Id { get; set; }
    public string Name { get; set; } = null!;
    public long MesoregionId { get; set; }

    //Relationships
    public List<Municipality> Municipalities { get; set; } = new List<Municipality>();
    public Mesoregion Mesoregion { get; set; } = null!;
    public MicroRegion() { }

    public MicroRegion(long itemId, string itemNome, long mesoregionId)
    {
        Id = itemId;
        Name = itemNome;
        MesoregionId = mesoregionId;
    }
}
