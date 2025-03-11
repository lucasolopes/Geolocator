namespace Elasticsearch.DTOs;

public class DistrictsDto
{
    public long Id { get; set; }
    public string Name { get; set; } = null!;
    public long MunicipalityId { get; set; }
}
