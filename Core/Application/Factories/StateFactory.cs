using Domain.Entities;

namespace Application.Factories;

public static class StateFactory
{
    public static State Create(int id, string name, string initials, int regionId)
    {
        name = name?.Trim() ?? throw new ArgumentNullException(nameof(name));
        initials = initials?.Trim() ?? throw new ArgumentNullException(nameof(initials));

        return new State(id, name, initials, regionId);
    }
}
