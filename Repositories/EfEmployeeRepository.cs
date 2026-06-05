using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AutoHub.Data;
using AutoHub.Models.Entities;

namespace AutoHub.Repositories;

public class EfEmployeeRepository : IEmployeeRepository
{
    private readonly AppDbContext _context;

    public EfEmployeeRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Employee>> GetAllAsync()
        => await _context.Employees.OrderByDescending(e => e.CreatedAt).ToListAsync();

    public async Task<Employee?> GetByIdAsync(int id)
        => await _context.Employees.FindAsync(id);

    public async Task AddAsync(Employee employee)
    {
        await _context.Employees.AddAsync(employee);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Employee employee)
    {
        _context.Employees.Update(employee);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var e = await _context.Employees.FindAsync(id);
        if (e is null) return;
        e.IsDeleted = true;
        e.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
    }
}
