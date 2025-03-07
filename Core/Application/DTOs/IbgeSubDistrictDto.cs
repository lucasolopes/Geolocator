using System.Text.Json.Serialization;

namespace Application.DTOs;

public class IbgeSubDistrictDto
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    
    [JsonPropertyName("nome")]
    public string Nome { get; set; } = null!;
    
    [JsonPropertyName("distrito")]
    public IbgeDistrictDto Distrito { get; set; } = null!;
}
