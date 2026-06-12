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
        public async Task<IActionResult> Cars(string? bodyStyle, string? fuelType, string? brand, string? searchTerm, string? sortOrder)
        {
            var vehicles = await _vehicleRepository.GetAllWithDetailsAsync("Auto", bodyStyle, fuelType);

            if (!string.IsNullOrWhiteSpace(brand))
                vehicles = vehicles.Where(v => v.Brand?.Name?.Equals(brand, StringComparison.OrdinalIgnoreCase) == true);

            if (!string.IsNullOrWhiteSpace(searchTerm))
                vehicles = vehicles.Where(v => v.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));

            switch (sortOrder)
            {
                case "price_desc":
                    vehicles = vehicles.OrderByDescending(v => v.CurrentPrice);
                    break;
                case "price_asc":
                    vehicles = vehicles.OrderBy(v => v.CurrentPrice);
                    break;
                case "name_desc":
                    vehicles = vehicles.OrderByDescending(v => v.Name);
                    break;
                case "name_asc":
                    vehicles = vehicles.OrderBy(v => v.Name);
                    break;
            }

            var dicts = await _dictService.GetAllAsync();
            ViewBag.FuelTypes = dicts.Where(d => d.Type == "FuelType").ToList();
            ViewBag.BodyStyles = dicts.Where(d => d.Type == "BodyStyle" && d.ParentCode == "Auto").ToList();
            ViewBag.BodyStyle = bodyStyle;
            ViewBag.FuelType = fuelType;
            ViewBag.Brand = brand;
            ViewBag.SearchTerm = searchTerm;
            ViewBag.SortOrder = sortOrder;
            
            // Get all vehicles of this type just for distinct dropdowns
            var allTypeVehicles = await _vehicleRepository.GetAllWithDetailsAsync("Auto", null, null);
            ViewBag.Brands = allTypeVehicles.Select(v => v.Brand?.Name).Where(n => n != null).Distinct().OrderBy(n => n).ToList();

            ViewData["Title"] = "Ô Tô - AutoHub";
            return View(vehicles);
        }

        // GET /moto
        [Route("moto")]
        public async Task<IActionResult> Moto(string? bodyStyle, string? fuelType, string? brand, string? searchTerm, string? sortOrder)
        {
            var vehicles = await _vehicleRepository.GetAllWithDetailsAsync("Motorbike", bodyStyle, fuelType);

            if (!string.IsNullOrWhiteSpace(brand))
                vehicles = vehicles.Where(v => v.Brand?.Name?.Equals(brand, StringComparison.OrdinalIgnoreCase) == true);

            if (!string.IsNullOrWhiteSpace(searchTerm))
                vehicles = vehicles.Where(v => v.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));

            switch (sortOrder)
            {
                case "price_desc":
                    vehicles = vehicles.OrderByDescending(v => v.CurrentPrice);
                    break;
                case "price_asc":
                    vehicles = vehicles.OrderBy(v => v.CurrentPrice);
                    break;
                case "name_desc":
                    vehicles = vehicles.OrderByDescending(v => v.Name);
                    break;
                case "name_asc":
                    vehicles = vehicles.OrderBy(v => v.Name);
                    break;
            }

            var dicts = await _dictService.GetAllAsync();
            ViewBag.FuelTypes = dicts.Where(d => d.Type == "FuelType").ToList();
            ViewBag.BodyStyles = dicts.Where(d => d.Type == "BodyStyle" && d.ParentCode == "Motorbike").ToList();
            ViewBag.BodyStyle = bodyStyle;
            ViewBag.FuelType = fuelType;
            ViewBag.Brand = brand;
            ViewBag.SearchTerm = searchTerm;
            ViewBag.SortOrder = sortOrder;
            
            var allTypeVehicles = await _vehicleRepository.GetAllWithDetailsAsync("Motorbike", null, null);
            ViewBag.Brands = allTypeVehicles.Select(v => v.Brand?.Name).Where(n => n != null).Distinct().OrderBy(n => n).ToList();

            ViewData["Title"] = "Xe Máy - AutoHub";
            return View(vehicles);
        }

        // GET /parts
        [Route("parts")]
        public async Task<IActionResult> Parts(string? brand, string? category, string? searchTerm, string? sortOrder)
        {
            var parts = await _sparePartRepository.GetAllWithDetailsAsync();

            if (!string.IsNullOrWhiteSpace(brand))
                parts = parts.Where(p => p.Brand?.Name?.Equals(brand, StringComparison.OrdinalIgnoreCase) == true);

            if (!string.IsNullOrWhiteSpace(category))
                parts = parts.Where(p => (p.CategoryMaster?.Name ?? p.Category)?.Contains(category, StringComparison.OrdinalIgnoreCase) == true);

            if (!string.IsNullOrWhiteSpace(searchTerm))
                parts = parts.Where(p => p.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));

            switch (sortOrder)
            {
                case "price_desc":
                    parts = parts.OrderByDescending(p => p.Price);
                    break;
                case "price_asc":
                    parts = parts.OrderBy(p => p.Price);
                    break;
                case "name_desc":
                    parts = parts.OrderByDescending(p => p.Name);
                    break;
                case "name_asc":
                    parts = parts.OrderBy(p => p.Name);
                    break;
            }

            ViewBag.Brand = brand;
            ViewBag.Category = category;
            ViewBag.SearchTerm = searchTerm;
            ViewBag.SortOrder = sortOrder;
            
            var allParts = await _sparePartRepository.GetAllWithDetailsAsync();
            ViewBag.Brands = allParts.Select(p => p.Brand?.Name).Where(n => n != null).Distinct().OrderBy(n => n).ToList();
            ViewBag.Categories = allParts.Select(p => p.CategoryMaster?.Name ?? p.Category).Where(n => !string.IsNullOrEmpty(n)).Distinct().OrderBy(n => n).ToList();

            ViewData["Title"] = "Phụ Tùng - AutoHub";
            return View(parts);
        }

        [HttpGet("api/catalog/suggestions")]
        public async Task<IActionResult> Suggestions(string term, string category = "Cars")
        {
            if (string.IsNullOrWhiteSpace(term))
                return Json(new List<string>());

            IEnumerable<string> suggestions = new List<string>();

            if (category == "All")
            {
                var vehicles = await _vehicleRepository.GetAllWithDetailsAsync(null, null, null);
                var parts = await _sparePartRepository.GetAllWithDetailsAsync();
                var vSugs = vehicles.Where(v => v.Name.Contains(term, StringComparison.OrdinalIgnoreCase)).Select(v => v.Name);
                var pSugs = parts.Where(p => p.Name.Contains(term, StringComparison.OrdinalIgnoreCase)).Select(p => p.Name);
                suggestions = vSugs.Concat(pSugs).Distinct().Take(8);
            }
            else if (category == "Cars" || category == "Moto")
            {
                var type = category == "Cars" ? "Auto" : "Motorbike";
                var vehicles = await _vehicleRepository.GetAllWithDetailsAsync(type, null, null);
                suggestions = vehicles.Where(v => v.Name.Contains(term, StringComparison.OrdinalIgnoreCase))
                                      .Select(v => v.Name)
                                      .Distinct()
                                      .Take(5);
            }
            else if (category == "Parts")
            {
                var parts = await _sparePartRepository.GetAllWithDetailsAsync();
                suggestions = parts.Where(p => p.Name.Contains(term, StringComparison.OrdinalIgnoreCase))
                                   .Select(p => p.Name)
                                   .Distinct()
                                   .Take(5);
            }

            return Json(suggestions);
        }
    }
}
