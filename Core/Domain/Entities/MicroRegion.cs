﻿namespace Domain.Entities;

public class MicroRegion
{
    public int Id { get; private set; }
    public string Name { get; private set; } = null!;
    public int MesoregionId { get; private set; }

    //Relationships
    public List<Municipality> Municipalities = new List<Municipality>();
    public Mesoregion Mesoregion = null!;

    public MicroRegion() { }

    public MicroRegion(int itemId, string itemNome, int mesoregionId)
    {
        Id = itemId;
        Name = itemNome;
        MesoregionId = mesoregionId;
    }
}
