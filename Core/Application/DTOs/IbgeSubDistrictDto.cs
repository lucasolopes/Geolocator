using System.Text.Json.Serialization;

namespace Application.DTOs;

public class IbgeSubDistrictDto
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = null!;
    
    [JsonPropertyName("nome")]
    public string Nome { get; set; } = null!;
    
    [JsonPropertyName("distrito")]
    public IbgeDistrictDto Distrito { get; set; } = null!;
}
