using Domain.Entities;

namespace Application.Factories;

public static class SubDistrictsFactory
{
    public static SubDistricts Create(int id, string name, string districtId)
    {
        name = name?.Trim() ?? throw new ArgumentNullException(nameof(name));
        districtId = districtId?.Trim() ?? throw new ArgumentNullException(nameof(districtId));

        return new SubDistricts(id, name, districtId);
    }
}
