using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AutoHub.Data;
using AutoHub.Models.Entities;

namespace AutoHub.Repositories;

public class EfShowroomRepository : IShowroomRepository
{
    private readonly AppDbContext _context;

    public EfShowroomRepository(AppDbContext context) => _context = context;

    public async Task<IEnumerable<Showroom>> GetAllAsync()
        => await _context.Showrooms.OrderBy(s => s.Name).ToListAsync();

    public async Task<IEnumerable<Showroom>> GetActiveAsync()
        => await _context.Showrooms.Where(s => s.IsActive).OrderBy(s => s.Name).ToListAsync();

    public async Task<Showroom?> GetByIdAsync(int id)
        => await _context.Showrooms.FindAsync(id);

    public async Task AddAsync(Showroom showroom)
    {
        await _context.Showrooms.AddAsync(showroom);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Showroom showroom)
    {
        _context.Showrooms.Update(showroom);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var s = await _context.Showrooms.FindAsync(id);
        if (s is null) return;
        s.IsDeleted  = true;
        s.UpdatedAt  = DateTime.UtcNow;
        await _context.SaveChangesAsync();
    }
}
