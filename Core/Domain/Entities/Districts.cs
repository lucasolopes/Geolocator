namespace Domain.Entities;

public class Districts
{
    public Districts() { }

    public Districts(long itemId, string itemNome, long municipalityId)
    {
        Id = itemId;
        Name = itemNome;
        MunicipalityId = municipalityId;
    }

    public long Id { get; set; }
    public string Name { get; set; } = null!;
    public long MunicipalityId { get; set; }

    public List<SubDistricts> SubDistricts { get; set; } = new();
    public Municipality Municipality { get; set; } = null!;
}
