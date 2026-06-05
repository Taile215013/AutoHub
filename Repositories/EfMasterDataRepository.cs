using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoHub.Data;
using AutoHub.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace AutoHub.Repositories;

public class EfMasterDataRepository : IMasterDataRepository
{
    private readonly AppDbContext _context;

    public EfMasterDataRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<VehicleName>> GetVehicleNamesAsync()
    {
        return await _context.VehicleNames
            .Include(vn => vn.Brand)
            .OrderBy(vn => vn.Brand.Name)
            .ThenBy(vn => vn.Name)
            .ToListAsync();
    }

    public async Task<List<VehicleVariant>> GetVehicleVariantsAsync(int? vehicleNameId = null)
    {
        var query = _context.VehicleVariants
            .Include(vv => vv.VehicleName)
            .ThenInclude(vn => vn.Brand)
            .AsQueryable();

        if (vehicleNameId.HasValue)
        {
            query = query.Where(vv => vv.VehicleNameId == vehicleNameId.Value);
        }

        return await query
            .OrderBy(vv => vv.VehicleName.Brand.Name)
            .ThenBy(vv => vv.VehicleName.Name)
            .ThenBy(vv => vv.Name)
            .ToListAsync();
    }

    public async Task<List<VehicleModelYear>> GetVehicleModelYearsAsync(int? vehicleVariantId = null)
    {
        var query = _context.VehicleModelYears
            .Include(vy => vy.VehicleVariant)
            .ThenInclude(vv => vv.VehicleName)
            .ThenInclude(vn => vn.Brand)
            .AsQueryable();

        if (vehicleVariantId.HasValue)
        {
            query = query.Where(vy => vy.VehicleVariantId == vehicleVariantId.Value);
        }

        return await query
            .OrderByDescending(vy => vy.Year)
            .ToListAsync();
    }

    public async Task<List<ProductCategory>> GetCategoriesByTypeAsync(string categoryType)
    {
        return await _context.ProductCategories
            .Where(c => c.CategoryType == categoryType)
            .OrderBy(c => c.Name)
            .ToListAsync();
    }

    public async Task<VehicleName> FindOrCreateVehicleNameAsync(int brandId, string name, string vehicleType, string? bodyStyle)
    {
        var cleanedName = (name ?? string.Empty).Trim();
        var normalizedName = Normalize(cleanedName);

        var existing = await _context.VehicleNames
            .FirstOrDefaultAsync(vn => vn.BrandId == brandId && vn.NormalizedName == normalizedName);

        if (existing != null)
        {
            return existing;
        }

        var vehicleName = new VehicleName
        {
            BrandId = brandId,
            Name = cleanedName,
            NormalizedName = normalizedName,
            VehicleType = vehicleType ?? string.Empty,
            BodyStyle = bodyStyle,
            CreatedAt = DateTime.UtcNow,
            IsDeleted = false
        };

        await _context.VehicleNames.AddAsync(vehicleName);
        await _context.SaveChangesAsync();

        return vehicleName;
    }

    public async Task<VehicleVariant?> FindOrCreateVehicleVariantAsync(int vehicleNameId, string? variantName, string? engineType, double? engineCapacity)
    {
        var cleanedName = variantName?.Trim();
        if (string.IsNullOrWhiteSpace(cleanedName))
        {
            return null;
        }

        var existing = await _context.VehicleVariants
            .FirstOrDefaultAsync(vv => vv.VehicleNameId == vehicleNameId && vv.Name == cleanedName);

        if (existing != null)
        {
            return existing;
        }

        var variant = new VehicleVariant
        {
            VehicleNameId = vehicleNameId,
            Name = cleanedName,
            EngineType = engineType,
            EngineCapacity = engineCapacity,
            CreatedAt = DateTime.UtcNow,
            IsDeleted = false
        };

        await _context.VehicleVariants.AddAsync(variant);
        await _context.SaveChangesAsync();

        return variant;
    }

    public async Task<VehicleModelYear?> FindOrCreateVehicleModelYearAsync(int vehicleVariantId, int? year)
    {
        if (!year.HasValue || year.Value <= 0)
        {
            return null;
        }

        var existing = await _context.VehicleModelYears
            .FirstOrDefaultAsync(vy => vy.VehicleVariantId == vehicleVariantId && vy.Year == year.Value);

        if (existing != null)
        {
            return existing;
        }

        var modelYear = new VehicleModelYear
        {
            VehicleVariantId = vehicleVariantId,
            Year = year.Value,
            CreatedAt = DateTime.UtcNow,
            IsDeleted = false
        };

        await _context.VehicleModelYears.AddAsync(modelYear);
        await _context.SaveChangesAsync();

        return modelYear;
    }

    public async Task<ProductCategory?> FindOrCreateCategoryAsync(string categoryType, string? nameOrCode)
    {
        var cleanedName = nameOrCode?.Trim();
        if (string.IsNullOrWhiteSpace(cleanedName))
        {
            return null;
        }

        var code = Normalize(cleanedName);
        var existing = await _context.ProductCategories
            .FirstOrDefaultAsync(c => c.CategoryType == categoryType && c.Code == code);

        if (existing != null)
        {
            return existing;
        }

        var category = new ProductCategory
        {
            CategoryType = categoryType,
            Name = cleanedName,
            Code = code,
            CreatedAt = DateTime.UtcNow,
            IsDeleted = false
        };

        await _context.ProductCategories.AddAsync(category);
        await _context.SaveChangesAsync();

        return category;
    }

    private static string Normalize(string value)
    {
        return string.Join(
            string.Empty,
            value.Trim().ToUpperInvariant().Where(c => !char.IsWhiteSpace(c)));
    }
}
