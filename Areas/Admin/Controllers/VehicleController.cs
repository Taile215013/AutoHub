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
        private readonly IMasterDataRepository _masterDataRepository;

        public VehicleController(
            IVehicleRepository vehicleRepository,
            IBrandRepository brandRepository,
            IFileService fileService,
            ISystemDictionaryService dictService,
            IMasterDataRepository masterDataRepository)
        {
            _vehicleRepository = vehicleRepository;
            _brandRepository = brandRepository;
            _fileService = fileService;
            _dictService = dictService;
            _masterDataRepository = masterDataRepository;
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
            ViewBag.VehicleNames = await _masterDataRepository.GetVehicleNamesAsync();

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(
            Vehicle vehicle,
            List<string> colors,
            IFormFile? uploadedFile,
            IFormFile? thumbnailFile,
            List<IFormFile>? additionalFiles,
            int? vehicleNameId,
            string? newVehicleName,
            string? variantName,
            int? modelYear)
        {
            var vehicleName = vehicleNameId.HasValue
                ? (await _masterDataRepository.GetVehicleNamesAsync()).FirstOrDefault(vn => vn.Id == vehicleNameId.Value)
                : null;

            if (vehicleName == null && !string.IsNullOrWhiteSpace(newVehicleName))
            {
                vehicleName = await _masterDataRepository.FindOrCreateVehicleNameAsync(
                    vehicle.BrandId,
                    newVehicleName,
                    vehicle.VehicleType,
                    vehicle.BodyStyle);
            }

            if (vehicleName != null)
            {
                vehicle.VehicleNameId = vehicleName.Id;
                vehicle.Name = vehicleName.Name;

                var variant = await _masterDataRepository.FindOrCreateVehicleVariantAsync(
                    vehicleName.Id,
                    variantName,
                    vehicle.EngineType,
                    vehicle.EngineCapacity);

                if (variant != null)
                {
                    vehicle.VehicleVariantId = variant.Id;
                    vehicle.Name = $"{vehicleName.Name} {variant.Name}";

                    var year = await _masterDataRepository.FindOrCreateVehicleModelYearAsync(variant.Id, modelYear);
                    if (year != null)
                    {
                        vehicle.VehicleModelYearId = year.Id;
                    }
                }
            }

            if (uploadedFile != null && uploadedFile.Length > 0)
            {
                vehicle.ImageUrl = await _fileService.UploadImageAsync(uploadedFile, "vehicles");
            }

            if (thumbnailFile != null && thumbnailFile.Length > 0)
            {
                vehicle.ThumbnailImageUrl = await _fileService.UploadThumbnailAsync(thumbnailFile, "vehicles");
            }

            if (additionalFiles != null && additionalFiles.Count > 0)
            {
                vehicle.AdditionalImages = await _fileService.UploadMultipleImagesAsync(additionalFiles, "vehicles");
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
            ViewBag.VehicleNames = await _masterDataRepository.GetVehicleNamesAsync();

            return View(vehicle);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(
            int id,
            Vehicle vehicle,
            List<string> colors,
            IFormFile? uploadedFile,
            IFormFile? thumbnailFile,
            List<IFormFile>? additionalFiles,
            int? vehicleNameId,
            string? newVehicleName,
            string? variantName,
            int? modelYear)
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

                if (thumbnailFile != null && thumbnailFile.Length > 0)
                {
                    existingVehicle.ThumbnailImageUrl = await _fileService.UploadThumbnailAsync(thumbnailFile, "vehicles");
                }

                if (additionalFiles != null && additionalFiles.Count > 0)
                {
                    existingVehicle.AdditionalImages = await _fileService.UploadMultipleImagesAsync(additionalFiles, "vehicles");
                }

                var vehicleName = vehicleNameId.HasValue
                    ? (await _masterDataRepository.GetVehicleNamesAsync()).FirstOrDefault(vn => vn.Id == vehicleNameId.Value)
                    : null;

                if (vehicleName == null && !string.IsNullOrWhiteSpace(newVehicleName))
                {
                    vehicleName = await _masterDataRepository.FindOrCreateVehicleNameAsync(
                        vehicle.BrandId,
                        newVehicleName,
                        vehicle.VehicleType,
                        vehicle.BodyStyle);
                }

                existingVehicle.Name = vehicle.Name;
                existingVehicle.VehicleNameId = vehicleName?.Id;
                existingVehicle.VehicleVariantId = null;
                existingVehicle.VehicleModelYearId = null;

                if (vehicleName != null)
                {
                    existingVehicle.Name = vehicleName.Name;
                    var variant = await _masterDataRepository.FindOrCreateVehicleVariantAsync(
                        vehicleName.Id,
                        variantName,
                        vehicle.EngineType,
                        vehicle.EngineCapacity);

                    if (variant != null)
                    {
                        existingVehicle.VehicleVariantId = variant.Id;
                        existingVehicle.Name = $"{vehicleName.Name} {variant.Name}";

                        var year = await _masterDataRepository.FindOrCreateVehicleModelYearAsync(variant.Id, modelYear);
                        existingVehicle.VehicleModelYearId = year?.Id;
                    }
                }

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
                ViewBag.VehicleNames = await _masterDataRepository.GetVehicleNamesAsync();

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
