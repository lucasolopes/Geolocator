namespace Domain.Entities;

public class Districts
{
    public int Id { get; private set; }
    public string Name { get; private set; }
    public int MunicipalityId { get; private set; }

    //Relationships
    public Municipality Municipality = null!;
}
