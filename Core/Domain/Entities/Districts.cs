namespace Domain.Entities;

public class Districts
{
    public int Id { get; private set; }
    public string Name { get; private set; } = null!;
    public int MunicipalityId { get; private set; }

    //Relationships
    public List<SubDistricts> SubDistricts = new List<SubDistricts>();
    public Municipality Municipality = null!;

    public Districts(int itemId, string itemNome, int municipalityId)
    {
        Id = itemId;
        Name = itemNome;
        MunicipalityId = municipalityId;
    }
}
