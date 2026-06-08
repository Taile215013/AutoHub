using AutoHub.Data;
using AutoHub.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace AutoHub.Repositories;

public class EfLocationRepository : ILocationRepository
{
    private readonly AppDbContext _db;
    public EfLocationRepository(AppDbContext db) => _db = db;

    public Task<IEnumerable<Province>> GetProvincesAsync() =>
        Task.FromResult<IEnumerable<Province>>(
            _db.Provinces.AsNoTracking().OrderBy(p => p.Code).ToList());

    public Task<IEnumerable<District>> GetDistrictsByProvinceCodeAsync(string provinceCode) =>
        Task.FromResult<IEnumerable<District>>(
            _db.Districts.AsNoTracking()
               .Where(d => d.ProvinceCode == provinceCode)
               .OrderBy(d => d.Name)
               .ToList());

    public Task<IEnumerable<Ward>> GetWardsByDistrictCodeAsync(string districtCode) =>
        Task.FromResult<IEnumerable<Ward>>(
            _db.Wards.AsNoTracking()
               .Where(w => w.DistrictCode == districtCode)
               .OrderBy(w => w.Name)
               .ToList());
}
