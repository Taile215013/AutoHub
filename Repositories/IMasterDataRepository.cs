using System.Collections.Generic;
using System.Threading.Tasks;
using AutoHub.Models.Entities;

namespace AutoHub.Repositories;

public interface IMasterDataRepository
{
    Task<List<VehicleName>> GetVehicleNamesAsync();

    Task<List<VehicleVariant>> GetVehicleVariantsAsync(int? vehicleNameId = null);

    Task<List<VehicleModelYear>> GetVehicleModelYearsAsync(int? vehicleVariantId = null);

    Task<List<ProductCategory>> GetCategoriesByTypeAsync(string categoryType);

    Task<VehicleName> FindOrCreateVehicleNameAsync(int brandId, string name, string vehicleType, string? bodyStyle);

    Task<VehicleVariant?> FindOrCreateVehicleVariantAsync(int vehicleNameId, string? variantName, string? engineType, double? engineCapacity);

    Task<VehicleModelYear?> FindOrCreateVehicleModelYearAsync(int vehicleVariantId, int? year);

    Task<ProductCategory?> FindOrCreateCategoryAsync(string categoryType, string? nameOrCode);
}
