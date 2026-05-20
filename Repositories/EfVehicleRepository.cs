using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AutoHub.Data;
using AutoHub.Models.Entities;

namespace AutoHub.Repositories;

public class EfVehicleRepository : IVehicleRepository
{
    private readonly AppDbContext _context;

    public EfVehicleRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Vehicle>> GetAllWithDetailsAsync(string? vehicleType, string? bodyStyle, string? fuelType)
    {
        var query = _context.Vehicles
            .Include(v => v.Brand)
            .Include(v => v.Colors)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(vehicleType))
        {
            query = query.Where(v => v.VehicleType == vehicleType);
        }

        if (!string.IsNullOrWhiteSpace(bodyStyle))
        {
            query = query.Where(v => v.BodyStyle == bodyStyle);
        }

        if (!string.IsNullOrWhiteSpace(fuelType))
        {
            query = query.Where(v => v.FuelType == fuelType);
        }

        return await query.ToListAsync();
    }

    public async Task<Vehicle?> GetByIdWithDetailsAsync(int id)
    {
        return await _context.Vehicles
            .Include(v => v.Brand)
            .Include(v => v.Colors)
            .FirstOrDefaultAsync(v => v.Id == id);
    }

    public async Task AddAsync(Vehicle vehicle)
    {
        await _context.Vehicles.AddAsync(vehicle);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Vehicle vehicle)
    {
        _context.Vehicles.Update(vehicle);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var vehicle = await _context.Vehicles.FindAsync(id);
        if (vehicle != null)
        {
            vehicle.IsDeleted = true;
            await _context.SaveChangesAsync();
        }
    }

    public async Task AddColorAsync(VehicleColor color)
    {
        await _context.VehicleColors.AddAsync(color);
        await _context.SaveChangesAsync();
    }
}
