using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AutoHub.Models.Entities;
using AutoHub.Repositories;
using AutoHub.Services;
using Microsoft.AspNetCore.Http;

namespace AutoHub.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class VehicleController : Controller
    {
        private readonly IVehicleRepository _vehicleRepository;
        private readonly IBrandRepository _brandRepository;
        private readonly IFileService _fileService;
        private readonly ISystemDictionaryService _dictService;

        public VehicleController(
            IVehicleRepository vehicleRepository,
            IBrandRepository brandRepository,
            IFileService fileService,
            ISystemDictionaryService dictService)
        {
            _vehicleRepository = vehicleRepository;
            _brandRepository = brandRepository;
            _fileService = fileService;
            _dictService = dictService;
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

            var dicts = await _dictService.GetAllAsync();
            ViewBag.VehicleTypes = dicts.Where(d => d.Type == "VehicleType").ToList();
            ViewBag.FuelTypes = dicts.Where(d => d.Type == "FuelType").ToList();
            ViewBag.Transmissions = dicts.Where(d => d.Type == "Transmission").ToList();
            ViewBag.VehicleColors = dicts.Where(d => d.Type == "VehicleColor").ToList();
            ViewBag.EngineTypes = dicts.Where(d => d.Type == "EngineType").ToList();
            ViewBag.BodyStyles = dicts.Where(d => d.Type == "BodyStyle").ToList();

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Vehicle vehicle, List<string> colors, IFormFile? uploadedFile)
        {
            if (uploadedFile != null && uploadedFile.Length > 0)
            {
                vehicle.ImageUrl = await _fileService.UploadImageAsync(uploadedFile, "vehicles");
            }

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

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var vehicle = await _vehicleRepository.GetByIdWithDetailsAsync(id);
            if (vehicle == null)
            {
                return NotFound();
            }

            var brands = await _brandRepository.GetAllWithDetailsAsync();
            ViewBag.Brands = brands.Where(b => b.IsVehicleBrand).ToList();

            var dicts = await _dictService.GetAllAsync();
            ViewBag.VehicleTypes = dicts.Where(d => d.Type == "VehicleType").ToList();
            ViewBag.FuelTypes = dicts.Where(d => d.Type == "FuelType").ToList();
            ViewBag.Transmissions = dicts.Where(d => d.Type == "Transmission").ToList();
            ViewBag.VehicleColors = dicts.Where(d => d.Type == "VehicleColor").ToList();
            ViewBag.EngineTypes = dicts.Where(d => d.Type == "EngineType").ToList();
            ViewBag.BodyStyles = dicts.Where(d => d.Type == "BodyStyle").ToList();

            return View(vehicle);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, Vehicle vehicle, List<string> colors, IFormFile? uploadedFile)
        {
            if (id != vehicle.Id) return BadRequest();

            try
            {
                var existingVehicle = await _vehicleRepository.GetByIdWithDetailsAsync(id);
                if (existingVehicle == null)
                {
                    return NotFound();
                }

                if (uploadedFile != null && uploadedFile.Length > 0)
                {
                    existingVehicle.ImageUrl = await _fileService.UploadImageAsync(uploadedFile, "vehicles");
                }

                existingVehicle.Name = vehicle.Name;
                existingVehicle.BrandId = vehicle.BrandId;
                existingVehicle.VehicleType = vehicle.VehicleType;
                existingVehicle.BodyStyle = vehicle.BodyStyle;
                existingVehicle.FuelType = vehicle.FuelType;
                existingVehicle.EngineType = vehicle.EngineType;
                existingVehicle.Transmission = vehicle.Transmission;
                existingVehicle.PurchasePrice = vehicle.PurchasePrice;
                existingVehicle.CurrentPrice = vehicle.CurrentPrice;
                existingVehicle.Quantity = vehicle.Quantity;
                existingVehicle.Description = vehicle.Description;
                existingVehicle.UpdatedAt = DateTime.UtcNow;

                await _vehicleRepository.UpdateAsync(existingVehicle);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Có lỗi xảy ra khi cập nhật: " + ex.Message);

                var brands = await _brandRepository.GetAllWithDetailsAsync();
                ViewBag.Brands = brands.Where(b => b.IsVehicleBrand).ToList();

                var dicts = await _dictService.GetAllAsync();
                ViewBag.VehicleTypes = dicts.Where(d => d.Type == "VehicleType").ToList();
                ViewBag.FuelTypes = dicts.Where(d => d.Type == "FuelType").ToList();
                ViewBag.Transmissions = dicts.Where(d => d.Type == "Transmission").ToList();
                ViewBag.VehicleColors = dicts.Where(d => d.Type == "VehicleColor").ToList();
                ViewBag.EngineTypes = dicts.Where(d => d.Type == "EngineType").ToList();
                ViewBag.BodyStyles = dicts.Where(d => d.Type == "BodyStyle").ToList();

                return View(vehicle);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _vehicleRepository.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
