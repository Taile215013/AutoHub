using AutoHub.Models.Entities;

namespace AutoHub.Repositories;

public interface ILocationRepository
{
    Task<IEnumerable<Province>> GetProvincesAsync();
    Task<IEnumerable<District>> GetDistrictsByProvinceCodeAsync(string provinceCode);
    Task<IEnumerable<Ward>>     GetWardsByDistrictCodeAsync(string districtCode);
}
