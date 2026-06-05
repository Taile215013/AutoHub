using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AutoHub.Models.Entities;
using AutoHub.Repositories;
using AutoHub.Services;

namespace AutoHub.Controllers
{
    public class CatalogController : Controller
    {
        private readonly IVehicleRepository _vehicleRepository;
        private readonly ISparePartRepository _sparePartRepository;
        private readonly ISystemDictionaryService _dictService;

        public CatalogController(
            IVehicleRepository vehicleRepository,
            ISparePartRepository sparePartRepository,
            ISystemDictionaryService dictService)
        {
            _vehicleRepository = vehicleRepository;
            _sparePartRepository = sparePartRepository;
            _dictService = dictService;
        }

        // GET /cars
        [Route("cars")]
        public async Task<IActionResult> Cars(string? bodyStyle, string? fuelType, string? brand)
        {
            var vehicles = await _vehicleRepository.GetAllWithDetailsAsync("Auto", bodyStyle, fuelType);

            if (!string.IsNullOrWhiteSpace(brand))
                vehicles = vehicles.Where(v => v.Brand?.Name?.Equals(brand, StringComparison.OrdinalIgnoreCase) == true);

            var dicts = await _dictService.GetAllAsync();
            ViewBag.FuelTypes = dicts.Where(d => d.Type == "FuelType").ToList();
            ViewBag.BodyStyle = bodyStyle;
            ViewBag.FuelType = fuelType;
            ViewBag.Brand = brand;
            ViewBag.Brands = vehicles.Select(v => v.Brand?.Name).Where(n => n != null).Distinct().OrderBy(n => n).ToList();

            ViewData["Title"] = "Ô Tô - AutoHub";
            return View(vehicles);
        }

        // GET /moto
        [Route("moto")]
        public async Task<IActionResult> Moto(string? bodyStyle, string? fuelType, string? brand)
        {
            var vehicles = await _vehicleRepository.GetAllWithDetailsAsync("Motorbike", bodyStyle, fuelType);

            if (!string.IsNullOrWhiteSpace(brand))
                vehicles = vehicles.Where(v => v.Brand?.Name?.Equals(brand, StringComparison.OrdinalIgnoreCase) == true);

            var dicts = await _dictService.GetAllAsync();
            ViewBag.FuelTypes = dicts.Where(d => d.Type == "FuelType").ToList();
            ViewBag.BodyStyle = bodyStyle;
            ViewBag.FuelType = fuelType;
            ViewBag.Brand = brand;
            ViewBag.Brands = vehicles.Select(v => v.Brand?.Name).Where(n => n != null).Distinct().OrderBy(n => n).ToList();

            ViewData["Title"] = "Xe Máy - AutoHub";
            return View(vehicles);
        }

        // GET /parts
        [Route("parts")]
        public async Task<IActionResult> Parts(string? brand, string? category)
        {
            var parts = await _sparePartRepository.GetAllWithDetailsAsync();

            if (!string.IsNullOrWhiteSpace(brand))
                parts = parts.Where(p => p.Brand?.Name?.Equals(brand, StringComparison.OrdinalIgnoreCase) == true);

            if (!string.IsNullOrWhiteSpace(category))
                parts = parts.Where(p => (p.CategoryMaster?.Name ?? p.Category)?.Contains(category, StringComparison.OrdinalIgnoreCase) == true);

            ViewBag.Brand = brand;
            ViewBag.Category = category;
            ViewBag.Brands = parts.Select(p => p.Brand?.Name).Where(n => n != null).Distinct().OrderBy(n => n).ToList();
            ViewBag.Categories = parts.Select(p => p.CategoryMaster?.Name ?? p.Category).Where(n => !string.IsNullOrEmpty(n)).Distinct().OrderBy(n => n).ToList();

            ViewData["Title"] = "Phụ Tùng - AutoHub";
            return View(parts);
        }
    }
}
