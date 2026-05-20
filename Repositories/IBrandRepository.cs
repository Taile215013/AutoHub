using System.Collections.Generic;
using System.Threading.Tasks;
using AutoHub.Models.Entities;

namespace AutoHub.Repositories;

public interface IBrandRepository
{
    Task<IEnumerable<Brand>> GetAllWithDetailsAsync();

    Task<Brand?> GetByIdAsync(int id);

    Task AddAsync(Brand brand);

    Task UpdateAsync(Brand brand);

    Task DeleteAsync(int id);

    Task<IEnumerable<Country>> GetAllCountriesAsync();
}
