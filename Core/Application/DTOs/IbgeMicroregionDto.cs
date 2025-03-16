using System.Text.Json.Serialization;

namespace Application.DTOs;

public class IbgeMicroregionDto
{
    [JsonPropertyName("id")] public long Id { get; set; }

    [JsonPropertyName("nome")] public string Nome { get; set; } = null!;

    [JsonPropertyName("mesorregiao")] public IbgeMesoregionDto Mesorregiao { get; set; } = null!;
}
