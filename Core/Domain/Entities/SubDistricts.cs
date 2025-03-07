namespace Domain.Entities;

public class SubDistricts
{
    public int Id { get; private set; }
    public string Name { get; private set; }
    public string DistrictId { get; private set; }

    //Relationships
    public Districts District = null!;
    
}
