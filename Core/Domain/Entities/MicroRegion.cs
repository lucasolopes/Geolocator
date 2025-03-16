namespace Domain.Entities;

public class MicroRegion
{
    public MicroRegion() { }

    public MicroRegion(long itemId, string itemNome, long mesoregionId)
    {
        Id = itemId;
        Name = itemNome;
        MesoregionId = mesoregionId;
    }

    public long Id { get; set; }
    public string Name { get; set; } = null!;
    public long MesoregionId { get; set; }

    public List<Municipality> Municipalities { get; set; } = new();
    public Mesoregion Mesoregion { get; set; } = null!;
}
