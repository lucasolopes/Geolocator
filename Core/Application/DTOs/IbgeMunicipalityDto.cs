using System.Text.Json.Serialization;

namespace Application.DTOs;

public class IbgeMunicipalityDto
{
    [JsonPropertyName("id")]
    public long Id { get; set; }
    
    [JsonPropertyName("nome")]
    public string Nome { get; set; } = null!;
    
    [JsonPropertyName("microrregiao")]
    public IbgeMicroregionDto Microrregiao { get; set; } = null!;
}
