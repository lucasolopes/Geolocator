namespace Elasticsearch.DTOs;

public class MicroRegionDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public int MesoregionId { get; set; }
}
