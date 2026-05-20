using System.Collections.Generic;
using System.Threading.Tasks;
using AutoHub.Models.Entities;

namespace AutoHub.Repositories;

public interface ISparePartRepository
{
    Task<IEnumerable<SparePart>> GetAllWithDetailsAsync();

    Task<SparePart?> GetByIdAsync(int id);

    Task AddAsync(SparePart sparePart);

    Task UpdateAsync(SparePart sparePart);

    Task DeleteAsync(int id);
}
