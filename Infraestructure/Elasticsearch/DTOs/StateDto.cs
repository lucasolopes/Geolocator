namespace Elasticsearch.DTOs;

public class StateDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Initials { get; set; } = null!;
    public int RegionId { get; set; }
}
