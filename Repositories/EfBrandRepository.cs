using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AutoHub.Data;
using AutoHub.Models.Entities;

namespace AutoHub.Repositories;

public class EfBrandRepository : IBrandRepository
{
    private readonly AppDbContext _context;

    public EfBrandRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Brand>> GetAllWithDetailsAsync()
    {
        return await _context.Brands
            .Include(b => b.Country)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Brand?> GetByIdAsync(int id)
    {
        return await _context.Brands
            .Include(b => b.Country)
            .FirstOrDefaultAsync(b => b.Id == id);
    }

    public async Task AddAsync(Brand brand)
    {
        brand.CreatedAt = DateTime.UtcNow;
        brand.IsDeleted = false;
        await _context.Brands.AddAsync(brand);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Brand brand)
    {
        brand.UpdatedAt = DateTime.UtcNow;
        _context.Brands.Update(brand);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var brand = await _context.Brands.FindAsync(id);
        if (brand != null)
        {
            brand.IsDeleted = true;
            brand.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<Country>> GetAllCountriesAsync()
    {
        return await _context.Countries
            .AsNoTracking()
            .ToListAsync();
    }
}
