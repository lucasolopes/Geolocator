namespace Elasticsearch.DTOs;

public class SubDistrictsDto
{
    public long Id { get; set; }
    public string Name { get; set; } = null!;
    public long DistrictId { get; set; }
}
