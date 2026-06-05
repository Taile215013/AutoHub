using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AutoHub.Models.Entities;
using AutoHub.Repositories;

namespace AutoHub.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CustomerController : Controller
    {
        private readonly IUserRepository _userRepository;

        public CustomerController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        // GET /Admin/Customer
        public async Task<IActionResult> Index()
        {
            var users = await _userRepository.GetAllAsync();
            return View(users);
        }

        // GET /Admin/Customer/Details/5
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) return NotFound();
            return View(user);
        }

        // GET /Admin/Customer/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) return NotFound();
            return View(user);
        }

        // POST /Admin/Customer/Edit/5
        [HttpPost]
        public async Task<IActionResult> Edit(int id, User model)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) return NotFound();

            user.FirstName   = model.FirstName;
            user.LastName    = model.LastName;
            user.Gender      = model.Gender;
            user.Email       = model.Email;
            user.PhoneNumber = model.PhoneNumber;
            user.HouseNumber = model.HouseNumber;
            user.StreetName  = model.StreetName;
            user.Ward        = model.Ward;
            user.District    = model.District;
            user.City        = model.City;
            user.RankLevel   = model.RankLevel;
            user.UpdatedAt   = DateTime.UtcNow;

            await _userRepository.UpdateAsync(user);
            return RedirectToAction(nameof(Index));
        }

        // POST /Admin/Customer/ToggleDisable/5
        [HttpPost]
        public async Task<IActionResult> ToggleDisable(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) return Json(new { success = false, message = "Không tìm thấy tài khoản!" });

            user.IsDeleted = !user.IsDeleted;
            user.UpdatedAt = DateTime.UtcNow;

            // Bypass global query filter for disabled users by detaching and re-attaching
            await _userRepository.UpdateAsync(user);
            return Json(new { success = true, isDisabled = user.IsDeleted });
        }

        // POST /Admin/Customer/Delete/5
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _userRepository.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
