using System.Collections.Generic;
using System.Threading.Tasks;
using AutoHub.Models.Entities;

namespace AutoHub.Repositories;

public interface IVehicleRepository
{
    Task<IEnumerable<Vehicle>> GetAllWithDetailsAsync(string? vehicleType, string? bodyStyle, string? fuelType);

    Task<Vehicle?> GetByIdWithDetailsAsync(int id);

    Task AddAsync(Vehicle vehicle);

    Task UpdateAsync(Vehicle vehicle);

    Task DeleteAsync(int id);

    Task AddColorAsync(VehicleColor color);
}
