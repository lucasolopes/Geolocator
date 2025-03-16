namespace Domain.Entities;

public class SubDistricts
{
    public SubDistricts()
    {
    }
    
    public SubDistricts(long id, string name, long districtId)
    {
        Id = id;
        Name = name;
        DistrictId = districtId;
    }

    public long Id { get; set; }
    public string Name { get; set; } = null!;
    public long DistrictId { get; set; }

    //Relationships
    public Districts District { get; set; } = null!;
}
