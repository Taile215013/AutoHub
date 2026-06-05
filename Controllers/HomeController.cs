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

        public HomeController(
            IVehicleRepository vehicleRepository, 
            IServiceRepository serviceRepository,
            ILocationService locationService,
            ISystemDictionaryService dictService)
        {
            _vehicleRepository = vehicleRepository;
            _serviceRepository = serviceRepository;
            _locationService = locationService;
            _dictService = dictService;
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

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
