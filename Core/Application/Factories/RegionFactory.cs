using Domain.Entities;

namespace Application.Factories;

public static class RegionFactory
{
    public static Region Create(int id, string name, string initials)
    { 
        name = name?.Trim() ?? throw new ArgumentNullException(nameof(name));
        initials = initials?.Trim() ?? throw new ArgumentNullException(nameof(initials));

        return new Region(id, name, initials);
    }
}
