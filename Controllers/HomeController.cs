using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AutoHub.Models;
using AutoHub.Models.Entities;
using AutoHub.Repositories;
using AutoHub.Services;

namespace AutoHub.Controllers
{
    public class HomeController : Controller
    {
        private readonly IVehicleRepository _vehicleRepository;
        private readonly IServiceRepository _serviceRepository;
        private readonly ILocationService _locationService;
        private readonly ISystemDictionaryService _dictService;
        private readonly ILocationRepository _locationRepository;

        public HomeController(
            IVehicleRepository vehicleRepository, 
            IServiceRepository serviceRepository,
            ILocationService locationService,
            ISystemDictionaryService dictService,
            ILocationRepository locationRepository)
        {
            _vehicleRepository  = vehicleRepository;
            _serviceRepository  = serviceRepository;
            _locationService    = locationService;
            _dictService        = dictService;
            _locationRepository = locationRepository;
        }

        public async Task<IActionResult> Index(string? vehicleType, string? bodyStyle, string? fuelType)
        {
            try
            {
                var vehicles = await _vehicleRepository.GetAllWithDetailsAsync(vehicleType, bodyStyle, fuelType);
                var services = await _serviceRepository.GetAllActiveAsync(null);

                var dicts = await _dictService.GetAllAsync();
                ViewBag.VehicleTypes = dicts.Where(d => d.Type == "VehicleType").ToList();
                ViewBag.FuelTypes = dicts.Where(d => d.Type == "FuelType").ToList();

                ViewBag.VehicleType = vehicleType;
                ViewBag.BodyStyle = bodyStyle;
                ViewBag.FuelType = fuelType;
                ViewBag.Services = services;
                ViewBag.Districts = _locationService.GetCities(); 

                return View(vehicles);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DB Error in Index: {ex.Message}");
                ViewBag.VehicleTypes = new List<SystemDictionary>();
                ViewBag.FuelTypes = new List<SystemDictionary>();
                ViewBag.Services = new List<Service>();
                ViewBag.Districts = new List<string>();
                return View(new List<Vehicle>());
            }
        }

        [HttpGet]
        public IActionResult GetCities()
        {
            var cities = _locationService.GetCities();
            return Json(cities);
        }

        [HttpGet]
        public IActionResult GetWards(string district)
        {
            var wards = _locationService.GetWards(district);
            return Json(wards);
        }

        [HttpGet]
        public IActionResult GetDistricts(string city)
        {
            var districts = _locationService.GetDistricts(city);
            return Json(districts);
        }

        // ── API địa chỉ mới — dùng DB Provinces/Districts/Wards ──────────────

        [HttpGet]
        public async Task<IActionResult> GetProvinces()
        {
            var provinces = await _locationRepository.GetProvincesAsync();
            return Json(provinces.Select(p => new { p.Code, p.Name }));
        }

        [HttpGet]
        public async Task<IActionResult> GetDistrictsByProvince(string provinceCode)
        {
            if (string.IsNullOrWhiteSpace(provinceCode)) return Json(Array.Empty<object>());
            var districts = await _locationRepository.GetDistrictsByProvinceCodeAsync(provinceCode);
            return Json(districts.Select(d => new { d.Code, d.Name }));
        }

        [HttpGet]
        public async Task<IActionResult> GetWardsByDistrict(string districtCode)
        {
            if (string.IsNullOrWhiteSpace(districtCode)) return Json(Array.Empty<object>());
            var wards = await _locationRepository.GetWardsByDistrictCodeAsync(districtCode);
            return Json(wards.Select(w => new { w.Code, w.Name }));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
