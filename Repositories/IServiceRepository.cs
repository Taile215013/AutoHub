using System.Collections.Generic;
using System.Threading.Tasks;
using AutoHub.Models.Entities;

namespace AutoHub.Repositories;

public interface IServiceRepository
{
    Task<IEnumerable<Service>> GetAllActiveAsync(string? vehicleType);

    Task<Service?> GetByIdAsync(int id);

    Task AddAsync(Service service);
}
