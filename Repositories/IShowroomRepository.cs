using System.Collections.Generic;
using System.Threading.Tasks;
using AutoHub.Models.Entities;

namespace AutoHub.Repositories;

public interface IShowroomRepository
{
    Task<IEnumerable<Showroom>> GetAllAsync();
    Task<IEnumerable<Showroom>> GetActiveAsync();
    Task<Showroom?> GetByIdAsync(int id);
    Task AddAsync(Showroom showroom);
    Task UpdateAsync(Showroom showroom);
    Task DeleteAsync(int id);
}
