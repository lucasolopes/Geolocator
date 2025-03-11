namespace Elasticsearch.DTOs;

public class MunicipalityDto
{
    public long Id { get; set; }
    public string Name { get; set; } = null!;
    public long MicroRegionId { get; set; }
}
