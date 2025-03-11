namespace Domain.Entities;

public class Districts
{
    public long Id { get; set; }
    public string Name { get; set; } = null!;
    public long MunicipalityId { get; set; }

    //Relationships
    public List<SubDistricts> SubDistricts = new List<SubDistricts>();
    public Municipality Municipality = null!;

    public Districts() { }

    public Districts(long itemId, string itemNome, long municipalityId)
    {
        Id = itemId;
        Name = itemNome;
        MunicipalityId = municipalityId;
    }
}
