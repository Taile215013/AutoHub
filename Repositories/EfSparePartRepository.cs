using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AutoHub.Data;
using AutoHub.Models.Entities;

namespace AutoHub.Repositories;

public class EfSparePartRepository : ISparePartRepository
{
    private readonly AppDbContext _context;

    public EfSparePartRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<SparePart>> GetAllWithDetailsAsync()
    {
        return await _context.SpareParts
            .Include(sp => sp.Brand)
            .Include(sp => sp.CategoryMaster)
            .Include(sp => sp.Compatibilities)
                .ThenInclude(c => c.VehicleName)
            .Include(sp => sp.Compatibilities)
                .ThenInclude(c => c.VehicleVariant)
            .Include(sp => sp.Compatibilities)
                .ThenInclude(c => c.VehicleModelYear)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<SparePart?> GetByIdAsync(int id)
    {
        return await _context.SpareParts
            .Include(sp => sp.Brand)
            .Include(sp => sp.CategoryMaster)
            .Include(sp => sp.Compatibilities)
                .ThenInclude(c => c.VehicleName)
            .Include(sp => sp.Compatibilities)
                .ThenInclude(c => c.VehicleVariant)
            .Include(sp => sp.Compatibilities)
                .ThenInclude(c => c.VehicleModelYear)
            .FirstOrDefaultAsync(sp => sp.Id == id);
    }

    public async Task AddAsync(SparePart sparePart)
    {
        sparePart.CreatedAt = DateTime.UtcNow;
        sparePart.IsDeleted = false;
        await _context.SpareParts.AddAsync(sparePart);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(SparePart sparePart)
    {
        sparePart.UpdatedAt = DateTime.UtcNow;
        _context.SpareParts.Update(sparePart);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var sparePart = await _context.SpareParts.FindAsync(id);
        if (sparePart != null)
        {
            sparePart.IsDeleted = true;
            sparePart.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }

    public async Task AddCompatibilityAsync(SparePartCompatibility compatibility)
    {
        compatibility.CreatedAt = DateTime.UtcNow;
        compatibility.IsDeleted = false;
        await _context.SparePartCompatibilities.AddAsync(compatibility);
        await _context.SaveChangesAsync();
    }
}
