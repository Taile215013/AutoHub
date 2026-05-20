using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AutoHub.Models.Entities;
using AutoHub.Repositories;

namespace AutoHub.Controllers
{
    public class AdminController : Controller
    {
        private readonly IVehicleRepository _vehicleRepository;
        private readonly IBrandRepository _brandRepository;
        private readonly ISparePartRepository _sparePartRepository;

        public AdminController(
            IVehicleRepository vehicleRepository,
            IBrandRepository brandRepository,
            ISparePartRepository sparePartRepository)
        {
            _vehicleRepository = vehicleRepository;
            _brandRepository = brandRepository;
            _sparePartRepository = sparePartRepository;
        }

        public async Task<IActionResult> Index()
        {
            var vehicles = await _vehicleRepository.GetAllWithDetailsAsync(null, null, null);
            return View(vehicles);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var brands = await _brandRepository.GetAllWithDetailsAsync();
            ViewBag.Brands = brands.Where(b => b.IsVehicleBrand).ToList();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Vehicle vehicle, List<string> colors)
        {
            vehicle.CreatedAt = DateTime.UtcNow;
            vehicle.IsDeleted = false;

            await _vehicleRepository.AddAsync(vehicle);

            if (colors != null && colors.Count > 0)
            {
                foreach (var colorName in colors)
                {
                    if (!string.IsNullOrWhiteSpace(colorName))
                    {
                        var color = new VehicleColor
                        {
                            VehicleId = vehicle.Id,
                            ColorName = colorName.Trim(),
                            CreatedAt = DateTime.UtcNow,
                            IsDeleted = false
                        };
                        await _vehicleRepository.AddColorAsync(color);
                    }
                }
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _vehicleRepository.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Brands()
        {
            var brands = await _brandRepository.GetAllWithDetailsAsync();
            return View(brands);
        }

        [HttpGet]
        public async Task<IActionResult> CreateBrand()
        {
            ViewBag.Countries = (await _brandRepository.GetAllCountriesAsync()).ToList();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateBrand(Brand brand)
        {
            brand.CreatedAt = DateTime.UtcNow;
            brand.IsDeleted = false;

            await _brandRepository.AddAsync(brand);
            return RedirectToAction(nameof(Brands));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteBrand(int id)
        {
            await _brandRepository.DeleteAsync(id);
            return RedirectToAction(nameof(Brands));
        }

        public async Task<IActionResult> SpareParts()
        {
            var parts = await _sparePartRepository.GetAllWithDetailsAsync();
            return View(parts);
        }

        [HttpGet]
        public async Task<IActionResult> CreateSparePart()
        {
            var brands = await _brandRepository.GetAllWithDetailsAsync();
            ViewBag.Brands = brands.Where(b => b.IsPartBrand).ToList();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateSparePart(SparePart sparePart)
        {
            if (sparePart.Price <= sparePart.CostPrice)
            {
                ModelState.AddModelError("Price", "Giá bán lẻ ra thị trường phải lớn hơn giá gốc nhập vào!");
                var brands = await _brandRepository.GetAllWithDetailsAsync();
                ViewBag.Brands = brands.Where(b => b.IsPartBrand).ToList();
                return View(sparePart);
            }

            sparePart.CreatedAt = DateTime.UtcNow;
            sparePart.IsDeleted = false;

            await _sparePartRepository.AddAsync(sparePart);
            return RedirectToAction(nameof(SpareParts));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteSparePart(int id)
        {
            await _sparePartRepository.DeleteAsync(id);
            return RedirectToAction(nameof(SpareParts));
        }
    }
}
