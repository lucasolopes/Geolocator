namespace Elasticsearch.DTOs;

public class MicroRegionDto
{
    public long Id { get; set; }
    public string Name { get; set; } = null!;
    public long MesoregionId { get; set; }
}
