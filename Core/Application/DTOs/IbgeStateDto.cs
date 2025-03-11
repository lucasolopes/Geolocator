using System.Text.Json.Serialization;

namespace Application.DTOs;

public class IbgeStateDto
{
    [JsonPropertyName("id")]
    public long Id { get; set; }
    
    [JsonPropertyName("nome")]
    public string Nome { get; set; } = null!;
    
    [JsonPropertyName("sigla")]
    public string Sigla { get; set; } = null!;
    
    [JsonPropertyName("regiao")]
    public IbgeRegionDto Regiao { get; set; } = null!;
}
