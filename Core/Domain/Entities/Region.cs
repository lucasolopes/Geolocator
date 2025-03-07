namespace Domain.Entities;

public class Region
{
    public Region() { }

    public Region(int id, string name, string initials)
    {
        Id = id;
        Name = name;
        Initials = initials;
    }

    public int Id { get; private set; }
    public string Name { get; private set; } = null!;
    public string Initials { get; private set; }
}
