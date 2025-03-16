using System.Text.Json.Serialization;

namespace Application.DTOs;

public class IbgeRegionDto
{
    public IbgeRegionDto(long id, string nome, string sigla)
    {
        Id = id;
        Nome = nome;
        Sigla = sigla;
    }

    [JsonPropertyName("id")] public long Id { get; set; }

    [JsonPropertyName("nome")] public string Nome { get; set; } = null!;

    [JsonPropertyName("sigla")] public string Sigla { get; set; } = null!;
}
