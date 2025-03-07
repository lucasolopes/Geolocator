namespace Domain.Entities;

public class SubDistricts
{
    public SubDistricts()
    {
    }
    
    public SubDistricts(int id, string name, int districtId)
    {
        Id = id;
        Name = name;
        DistrictId = districtId;
    }

    public int Id { get; private set; }
    public string Name { get; private set; } = null!;
    public int DistrictId { get; private set; }

    //Relationships
    public Districts District = null!;
}
