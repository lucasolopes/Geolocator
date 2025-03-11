namespace Elasticsearch.DTOs;

public class StateDto
{
    public long Id { get; set; }
    public string Name { get; set; } = null!;
    public string Initials { get; set; } = null!;
    public long RegionId { get; set; }
}
