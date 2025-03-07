namespace Domain.Entities;

public class Municipality
{
    // Construtor sem parâmetros para desserialização
    public Municipality() { }

    // Construtor para uso normal
    public Municipality(int id, string name, int microRegionId)
    {
        Id = id;
        Name = name;
        MicroRegionId = microRegionId;
    }

    // Propriedades com getters públicos e setters privados ou públicos
    public int Id { get; private set; }
    public string Name { get; private set; } = null!;
    public int MicroRegionId { get; private set; }

    // Relacionamentos (que o Elasticsearch não precisa)
    public List<Districts> Districts { get; private set; } = new List<Districts>();
    public MicroRegion MicroRegion { get; private set; } = null!;
}
