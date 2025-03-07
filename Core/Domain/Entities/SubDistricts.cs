namespace Domain.Entities;

public class SubDistricts
{
    public int Id { get; private set; }
    public string Name { get; private set; } = null!;
    public string DistrictId { get; private set; } = null!;

    //Relationships
    public Districts District = null!;

    public SubDistricts() { }

    public SubDistricts(int itemId, string itemNome, string toString)
    {
        Id = itemId;
        Name = itemNome;
        DistrictId = toString;
    }
}
