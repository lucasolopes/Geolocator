using Domain.Entities;

namespace Application.Factories;

public static class RegionFactory
{
    public static Region Create(int id, string name, string initials)
    {
        if (id <= 0)
        {
            throw new ArgumentException("Id must be greater than zero", nameof(id));
        }

        name = name?.Trim() ?? throw new ArgumentNullException(nameof(name));
        initials = initials?.Trim() ?? throw new ArgumentNullException(nameof(initials));

        return new Region(id, name, initials);  // Usando construtor em vez de inicialização de objeto
    }
}
