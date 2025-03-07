namespace Elasticsearch.DTOs;

public class MunicipalityDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public int MicroRegionId { get; set; }
}
