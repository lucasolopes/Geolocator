﻿namespace Domain.Entities;

public class Mesoregion
{
    public long Id { get; set; }
    public string Name { get; set; } = null!;
    public long StateId { get; set; }

    //Relationships
    public List<MicroRegion> MicroRegions = new List<MicroRegion>();
    public State State = null!;

    public Mesoregion() { }

    public Mesoregion(long itemId, string itemNome, long stateId)
    {
        Id = itemId;
        Name = itemNome;
        StateId = stateId;
    }
}
