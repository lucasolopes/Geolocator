using System.Text.Json.Serialization;

namespace Application.DTOs;

public class IbgeMesoregionDto
{
    [JsonPropertyName("id")]
    public long Id { get; set; }
    
    [JsonPropertyName("nome")]
    public string Nome { get; set; } = null!;
    
    [JsonPropertyName("UF")]
    public IbgeStateDto UF { get; set; } = null!;
}
