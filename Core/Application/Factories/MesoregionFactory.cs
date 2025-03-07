using Domain.Entities;

namespace Application.Factories;

public static class MesoregionFactory
{
    public static Mesoregion Create(int id, string name, int stateId)
    { 
        name = name?.Trim() ?? throw new ArgumentNullException(nameof(name));

        return new Mesoregion(id, name, stateId);
    }
}
