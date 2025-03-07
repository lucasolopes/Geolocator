using Domain.Entities;

namespace Application.Factories;

public static class MicroRegionFactory
{
    public static MicroRegion Create(int id, string name, int mesoregionId)
    { 
        name = name?.Trim() ?? throw new ArgumentNullException(nameof(name));

        return new MicroRegion(id, name, mesoregionId);
    }
}
