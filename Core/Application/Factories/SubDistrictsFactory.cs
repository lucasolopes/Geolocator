using Domain.Entities;

namespace Application.Factories;

public static class SubDistrictsFactory
{
    public static SubDistricts Create(int id, string name, int districtId)
    {
        name = name?.Trim() ?? throw new ArgumentNullException(nameof(name));

        return new SubDistricts(id, name, districtId);
    }
}
