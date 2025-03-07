using System.Text.Json.Serialization;

namespace Application.DTOs;

public class IbgeDistrictDto
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    
    [JsonPropertyName("nome")]
    public string Nome { get; set; } = null!;
    
    [JsonPropertyName("municipio")]
    public IbgeMunicipalityDto Municipio { get; set; } = null!;
}
