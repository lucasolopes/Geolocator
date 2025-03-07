namespace Domain.Entities;

public class SubDistricts
{
    public int Id { get; private set; }
    public string Name { get; private set; } = null!;
    public string DistrictId { get; private set; } = null!;

    //Relationships
    public Districts District = null!;
    
}
