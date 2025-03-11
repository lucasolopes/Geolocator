namespace Domain.Entities;

public class Municipality
{
    // Construtor sem parâmetros para desserialização
    public Municipality() { }

    // Construtor para uso normal
    public Municipality(long id, string name, long microRegionId)
    {
        Id = id;
        Name = name;
        MicroRegionId = microRegionId;
    }

    // Propriedades com getters públicos e setters privados ou públicos
    public long Id { get; set; }
    public string Name { get; set; } = null!;
    public long MicroRegionId { get; set; }

    // Relacionamentos (que o Elasticsearch não precisa)
    public List<Districts> Districts { get; set; } = new List<Districts>();
    public MicroRegion MicroRegion { get; set; } = null!;
}
