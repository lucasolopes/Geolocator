using Domain.Entities;

namespace Application.Factories;

public static class DistrictsFactory
{
    public static Districts Create(int id, string name, int municipalityId)
    { 
        name = name?.Trim() ?? throw new ArgumentNullException(nameof(name));

        return new Districts(id, name, municipalityId);
    }
}
