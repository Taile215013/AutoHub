using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using AutoHub.Models.Entities;
using AutoHub.Repositories;
using AutoHub.Services;

namespace AutoHub.Areas.Admin.Controllers;

[Area("Admin")]
public class EmployeeController : Controller
{
    private readonly IEmployeeRepository _repo;
    private readonly IEmployeeService    _service;
    private readonly ISystemDictionaryService _dictService;
    private readonly ILocationService    _locationService;

    public EmployeeController(
        IEmployeeRepository repo,
        IEmployeeService service,
        ISystemDictionaryService dictService,
        ILocationService locationService)
    {
        _repo            = repo;
        _service         = service;
        _dictService     = dictService;
        _locationService = locationService;
    }

    // GET /Admin/Employee
    public async Task<IActionResult> Index()
    {
        var employees = await _repo.GetAllAsync();
        ViewBag.Positions = await _dictService.GetDictionariesByTypeAsync("EmployeePosition");
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
        await _service.CreateAsync(employee, thumbnailFile, imageFiles);
        return RedirectToAction(nameof(Index));
    }

    // GET /Admin/Employee/Edit/5
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var emp = await _repo.GetByIdAsync(id);
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
        var existing = await _repo.GetByIdAsync(id);
        if (existing == null) return NotFound();

        employee.DateOfBirth = new DateTime(dobYear, dobMonth, dobDay);
        await _service.UpdateAsync(existing, employee, thumbnailFile, imageFiles);
        return RedirectToAction(nameof(Index));
    }

    // POST /Admin/Employee/Delete/5
    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        await _repo.DeleteAsync(id);
        return RedirectToAction(nameof(Index));
    }

    // GET /Admin/Employee/Details/5
    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var emp = await _repo.GetByIdAsync(id);
        if (emp == null) return NotFound();
        return View(emp);
    }

    // ── Private helpers ────────────────────────────────────────────────────

    private async Task PopulateViewBag(Employee? emp = null)
    {
        ViewBag.Positions = await _dictService.GetDictionariesByTypeAsync("EmployeePosition");
        ViewBag.Cities    = _locationService.GetCities();
        ViewBag.Districts = emp != null ? _locationService.GetDistricts(emp.City) : new List<string>();
        ViewBag.Wards     = emp != null ? _locationService.GetWards(emp.District ?? "") : new List<string>();
        ViewBag.ExistingImages = emp?.GetImageList() ?? new List<string>();
    }
}
