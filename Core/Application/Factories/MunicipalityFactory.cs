using Domain.Entities;

namespace Application.Factories;

public static class MunicipalityFactory
{
    public static Municipality Create(int id, string name, int microRegionId)
    { 
        name = name?.Trim() ?? throw new ArgumentNullException(nameof(name));

        return new Municipality(id, name, microRegionId);
    }
}
