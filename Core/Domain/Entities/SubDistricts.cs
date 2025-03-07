namespace Domain.Entities;

public class SubDistricts
{
    public SubDistricts()
    {
    }
    
    public SubDistricts(int id, string name, string districtId)
    {
        Id = id;
        Name = name;
        DistrictId = districtId;
    }

    public int Id { get; private set; }
    public string Name { get; private set; } = null!;
    public string DistrictId { get; private set; } = null!;

    //Relationships
    public Districts District = null!;
}
