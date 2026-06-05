using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using AutoHub.Models.Entities;
using AutoHub.Repositories;
using AutoHub.Services;

namespace AutoHub.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SparePartController : Controller
    {
        private readonly ISparePartRepository _sparePartRepository;
        private readonly IBrandRepository _brandRepository;
        private readonly ISystemDictionaryService _dictService;
        private readonly IFileService _fileService;
        private readonly IMasterDataRepository _masterDataRepository;

        public SparePartController(
            ISparePartRepository sparePartRepository,
            IBrandRepository brandRepository,
            ISystemDictionaryService dictService,
            IFileService fileService,
            IMasterDataRepository masterDataRepository)
        {
            _sparePartRepository = sparePartRepository;
            _brandRepository = brandRepository;
            _dictService = dictService;
            _fileService = fileService;
            _masterDataRepository = masterDataRepository;
        }

        public async Task<IActionResult> Index()
        {
            var parts = await _sparePartRepository.GetAllWithDetailsAsync();
            return View(parts);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var brands = await _brandRepository.GetAllWithDetailsAsync();
            ViewBag.Brands = brands.Where(b => b.IsPartBrand).ToList();
            ViewBag.Categories = await _dictService.GetDictionariesByTypeAsync("SparePartCategory");
            ViewBag.MasterCategories = await _masterDataRepository.GetCategoriesByTypeAsync("SparePart");
            ViewBag.VehicleNames = await _masterDataRepository.GetVehicleNamesAsync();
            ViewBag.VehicleVariants = await _masterDataRepository.GetVehicleVariantsAsync();
            ViewBag.VehicleModelYears = await _masterDataRepository.GetVehicleModelYearsAsync();

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(
            SparePart sparePart,
            IFormFile? ImageFile,
            IFormFile? thumbnailFile,
            List<IFormFile>? additionalFiles,
            int? compatibleVehicleNameId,
            int? compatibleVehicleVariantId,
            int? compatibleVehicleModelYearId)
        {
            if (sparePart.Price <= sparePart.CostPrice)
            {
                ModelState.AddModelError("Price", "Giá bán lẻ ra thị trường phải lớn hơn giá gốc nhập vào!");

                var brands = await _brandRepository.GetAllWithDetailsAsync();
                ViewBag.Brands = brands.Where(b => b.IsPartBrand).ToList();
                ViewBag.Categories = await _dictService.GetDictionariesByTypeAsync("SparePartCategory");
                ViewBag.MasterCategories = await _masterDataRepository.GetCategoriesByTypeAsync("SparePart");
                ViewBag.VehicleNames = await _masterDataRepository.GetVehicleNamesAsync();
                ViewBag.VehicleVariants = await _masterDataRepository.GetVehicleVariantsAsync();
                ViewBag.VehicleModelYears = await _masterDataRepository.GetVehicleModelYearsAsync();

                return View(sparePart);
            }

            var category = await _masterDataRepository.FindOrCreateCategoryAsync("SparePart", sparePart.Category);
            sparePart.CategoryId = category?.Id;

            if (ImageFile != null && ImageFile.Length > 0)
            {
                sparePart.ImageUrl = await _fileService.UploadImageAsync(ImageFile, "spareparts");
            }

            if (thumbnailFile != null && thumbnailFile.Length > 0)
            {
                sparePart.ThumbnailImageUrl = await _fileService.UploadThumbnailAsync(thumbnailFile, "spareparts");
            }

            if (additionalFiles != null && additionalFiles.Count > 0)
            {
                sparePart.AdditionalImages = await _fileService.UploadMultipleImagesAsync(additionalFiles, "spareparts");
            }

            sparePart.CreatedAt = DateTime.UtcNow;
            sparePart.IsDeleted = false;

            await _sparePartRepository.AddAsync(sparePart);

            if (compatibleVehicleNameId.HasValue)
            {
                await _sparePartRepository.AddCompatibilityAsync(new SparePartCompatibility
                {
                    SparePartId = sparePart.Id,
                    VehicleNameId = compatibleVehicleNameId.Value,
                    VehicleVariantId = compatibleVehicleVariantId,
                    VehicleModelYearId = compatibleVehicleModelYearId
                });
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _sparePartRepository.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
