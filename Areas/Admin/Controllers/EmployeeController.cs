using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using AutoHub.Models.Entities;
using AutoHub.Repositories;
using AutoHub.Services;

namespace AutoHub.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class EmployeeController : Controller
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly ISystemDictionaryService _dictService;
        private readonly IFileService _fileService;
        private readonly ILocationService _locationService;

        public EmployeeController(
            IEmployeeRepository employeeRepository,
            ISystemDictionaryService dictService,
            IFileService fileService,
            ILocationService locationService)
        {
            _employeeRepository = employeeRepository;
            _dictService = dictService;
            _fileService = fileService;
            _locationService = locationService;
        }

        // GET /Admin/Employee
        public async Task<IActionResult> Index()
        {
            var employees = await _employeeRepository.GetAllAsync();
            var positions = await _dictService.GetDictionariesByTypeAsync("EmployeePosition");
            ViewBag.Positions = positions;
            return View(employees);
        }

        // GET /Admin/Employee/Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await PopulateViewBag();
            return View(new Employee());
        }

        // POST /Admin/Employee/Create
        [HttpPost]
        public async Task<IActionResult> Create(
            Employee employee,
            IFormFile? thumbnailFile,
            List<IFormFile>? imageFiles,
            int dobDay, int dobMonth, int dobYear)
        {
            employee.DateOfBirth = new DateTime(dobYear, dobMonth, dobDay);

            if (thumbnailFile != null && thumbnailFile.Length > 0)
                employee.ThumbnailImageUrl = await _fileService.UploadThumbnailAsync(thumbnailFile, "employees");

            if (imageFiles != null && imageFiles.Count > 0)
            {
                var urls = new List<string>();
                foreach (var file in imageFiles)
                {
                    if (file.Length > 0)
                        urls.Add(await _fileService.UploadImageAsync(file, "employees"));
                }
                employee.ImageUrl = JsonSerializer.Serialize(urls);
            }

            employee.CreatedAt = DateTime.UtcNow;
            employee.IsDeleted = false;
            await _employeeRepository.AddAsync(employee);
            return RedirectToAction(nameof(Index));
        }

        // GET /Admin/Employee/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var emp = await _employeeRepository.GetByIdAsync(id);
            if (emp == null) return NotFound();
            await PopulateViewBag(emp);
            return View(emp);
        }

        // POST /Admin/Employee/Edit/5
        [HttpPost]
        public async Task<IActionResult> Edit(
            int id,
            Employee employee,
            IFormFile? thumbnailFile,
            List<IFormFile>? imageFiles,
            int dobDay, int dobMonth, int dobYear)
        {
            var existing = await _employeeRepository.GetByIdAsync(id);
            if (existing == null) return NotFound();

            existing.FirstName = employee.FirstName;
            existing.LastName = employee.LastName;
            existing.Gender = employee.Gender;
            existing.DateOfBirth = new DateTime(dobYear, dobMonth, dobDay);
            existing.PhoneNumber = employee.PhoneNumber;
            existing.Email = employee.Email;
            existing.NationalId = employee.NationalId;
            existing.HouseNumber = employee.HouseNumber;
            existing.StreetName = employee.StreetName;
            existing.Ward = employee.Ward;
            existing.District = employee.District;
            existing.City = employee.City;
            existing.HeightCm = employee.HeightCm;
            existing.WeightKg = employee.WeightKg;
            existing.Position = employee.Position;
            existing.BaseSalary = employee.BaseSalary;
            existing.IsActive = employee.IsActive;
            existing.Notes = employee.Notes;
            existing.UpdatedAt = DateTime.UtcNow;

            if (thumbnailFile != null && thumbnailFile.Length > 0)
                existing.ThumbnailImageUrl = await _fileService.UploadThumbnailAsync(thumbnailFile, "employees");

            if (imageFiles != null && imageFiles.Count > 0)
            {
                var urls = new List<string>();
                // Giữ ảnh cũ nếu có
                if (!string.IsNullOrEmpty(existing.ImageUrl))
                {
                    try { urls.AddRange(JsonSerializer.Deserialize<List<string>>(existing.ImageUrl) ?? new()); }
                    catch { }
                }
                foreach (var file in imageFiles)
                {
                    if (file.Length > 0)
                        urls.Add(await _fileService.UploadImageAsync(file, "employees"));
                }
                existing.ImageUrl = JsonSerializer.Serialize(urls);
            }

            await _employeeRepository.UpdateAsync(existing);
            return RedirectToAction(nameof(Index));
        }

        // POST /Admin/Employee/Delete/5
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _employeeRepository.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        // GET /Admin/Employee/Details/5
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var emp = await _employeeRepository.GetByIdAsync(id);
            if (emp == null) return NotFound();
            return View(emp);
        }

        // ── private helpers ──────────────────────────────────────────────────
        private async Task PopulateViewBag(Employee? emp = null)
        {
            var positions = await _dictService.GetDictionariesByTypeAsync("EmployeePosition");
            ViewBag.Positions = positions;
            ViewBag.Cities = _locationService.GetCities();

            if (emp != null)
            {
                ViewBag.Districts = _locationService.GetDistricts(emp.City);
                ViewBag.Wards = _locationService.GetWards(emp.District ?? "");
                ViewBag.ExistingImages = string.IsNullOrEmpty(emp.ImageUrl)
                    ? new List<string>()
                    : (JsonSerializer.Deserialize<List<string>>(emp.ImageUrl) ?? new());
            }
            else
            {
                ViewBag.Districts = new List<string>();
                ViewBag.Wards = new List<string>();
                ViewBag.ExistingImages = new List<string>();
            }
        }
    }
}
