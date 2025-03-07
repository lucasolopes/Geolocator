namespace Elasticsearch.Options;

public class ElasticsearchOptions
{
    public string Uri { get; set; } = null!;
    public string RegionIndexName { get; set; } = "regions";
    public string StateIndexName { get; set; } = "states";
    public string MesoregionIndexName { get; set; } = "mesoregions";
    public string MicroRegionIndexName { get; set; } = "microregions";
    public string MunicipalityIndexName { get; set; } = "municipalities";
    public string DistrictIndexName { get; set; } = "districts";
    public string SubDistrictIndexName { get; set; } = "subdistricts";
}
