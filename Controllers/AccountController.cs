using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using AutoHub.Models.Entities;
using AutoHub.Repositories;
using AutoHub.Services;

namespace AutoHub.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly IAuthService _authService;
        private readonly IVehicleRepository _vehicleRepository;
        private readonly IOrderRepository _orderRepository;

        public AccountController(
            IUserRepository userRepository,
            IAuthService authService,
            IVehicleRepository vehicleRepository,
            IOrderRepository orderRepository)
        {
            _userRepository = userRepository;
            _authService = authService;
            _vehicleRepository = vehicleRepository;
            _orderRepository = orderRepository;
        }

        // ---------- XÁC THỰC (AUTHENTICATION) ---------- //

        [HttpGet]
        public IActionResult Login()
        {
            if (HttpContext.Session.GetInt32("UserId") != null)
                return RedirectToAction("Account", "Account");
                
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string loginInput, string password)
        {
            try
            {
                var (success, user, message) = await _authService.LoginAsync(loginInput, password);
                if (!success)
                {
                    ViewBag.Error = message;
                    return View();
                }

                _authService.SetUserSession(HttpContext.Session, user!);
                return RedirectToAction("Account", "Account");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DB Error in Login: {ex.Message}");
                ViewBag.Error = "Hệ thống đang bảo trì, vui lòng thử lại sau.";
                return View();
            }
        }

        [HttpGet]
        public IActionResult Register()
        {
            if (HttpContext.Session.GetInt32("UserId") != null)
                return RedirectToAction("Account", "Account");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(User user, string Password, string ConfirmPassword)
        {
            try
            {
                var (success, message) = await _authService.RegisterAsync(user, Password, ConfirmPassword);
                if (!success)
                {
                    ViewBag.Error = message;
                    return View(user);
                }

                _authService.SetUserSession(HttpContext.Session, user);
                return RedirectToAction("Account", "Account");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DB Error in Register: {ex.Message}");
                ViewBag.Error = "Lỗi hệ thống khi đăng ký. Vui lòng thử lại.";
                return View(user);
            }
        }

        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        // ---------- TRANG CÁ NHÂN ---------- //

        public async Task<IActionResult> Account()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            try
            {
                var user = await _userRepository.GetByIdAsync(userId.Value);
                if (user == null)
                {
                    HttpContext.Session.Clear();
                    return RedirectToAction("Login", "Account");
                }

                var orders = await _orderRepository.GetOrdersByUserIdAsync(userId.Value);

                ViewBag.Orders = orders;
                ViewBag.Vehicles = await _vehicleRepository.GetAllWithDetailsAsync(null, null, null);

                return View(user);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DB Error in Account: {ex.Message}");
                
                var emptyUser = new User 
                { 
                    FirstName = HttpContext.Session.GetString("UserName") ?? "Thành viên",
                    LastName = "",
                    Email = "Đang bảo trì DB",
                    PhoneNumber = "Đang bảo trì DB",
                    RankLevel = "Bronze"
                };
                ViewBag.Orders = new List<Order>();
                ViewBag.Vehicles = new List<Vehicle>();
                
                ViewBag.Warning = "Hệ thống cơ sở dữ liệu đang bảo trì. Một số thông tin có thể không khả dụng.";
                return View(emptyUser);
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateAddress(string City, string District, string Ward, string HouseNumber)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Account");

            try
            {
                var user = await _userRepository.GetByIdAsync(userId.Value);
                if (user != null)
                {
                    user.City = City;
                    user.District = District;
                    user.Ward = Ward;
                    user.HouseNumber = HouseNumber;
                    await _userRepository.UpdateAsync(user);
                    TempData["Success"] = "Cập nhật địa chỉ giao nhận thành công!";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating address: {ex.Message}");
            }

            return RedirectToAction("Account", "Account");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProfile(
            string FirstName, string LastName, string Gender,
            DateTime? DateOfBirth, string? Email, string? PhoneNumber)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return Json(new { success = false, message = "Phiên đăng nhập đã hết hạn, vui lòng đăng nhập lại!" });

            try
            {
                var user = await _userRepository.GetByIdAsync(userId.Value);
                if (user == null)
                    return Json(new { success = false, message = "Tài khoản không tồn tại!" });

                if (!string.IsNullOrWhiteSpace(Email) && Email != user.Email)
                {
                    if (await _userRepository.IsEmailTakenAsync(Email, user.Id))
                        return Json(new { success = false, message = "Email này đã được sử dụng bởi tài khoản khác!" });
                }

                if (!string.IsNullOrWhiteSpace(PhoneNumber) && PhoneNumber != user.PhoneNumber)
                {
                    if (await _userRepository.IsPhoneTakenAsync(PhoneNumber, user.Id))
                        return Json(new { success = false, message = "Số điện thoại này đã được sử dụng bởi tài khoản khác!" });
                }

                user.FirstName = FirstName?.Trim() ?? user.FirstName;
                user.LastName = LastName?.Trim() ?? user.LastName;
                user.Gender = Gender ?? user.Gender;
                user.DateOfBirth = DateOfBirth;
                user.Email = string.IsNullOrWhiteSpace(Email) ? user.Email : Email.Trim();
                user.PhoneNumber = string.IsNullOrWhiteSpace(PhoneNumber) ? user.PhoneNumber : PhoneNumber.Trim();
                user.UpdatedAt = DateTime.UtcNow;

                await _userRepository.UpdateAsync(user);

                HttpContext.Session.SetString("UserName", user.FirstName);

                return Json(new
                {
                    success = true,
                    message = "Cập nhật thông tin thành công!",
                    data = new
                    {
                        fullName = $"{user.LastName} {user.FirstName}",
                        email = user.Email,
                        phoneNumber = user.PhoneNumber,
                        gender = user.Gender,
                        dateOfBirth = user.DateOfBirth?.ToString("dd/MM/yyyy") ?? "Chưa thiết lập",
                        avatarChar = user.FirstName?[0].ToString().ToUpper() ?? "U"
                    }
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating profile: {ex.Message}");
                return Json(new { success = false, message = "Có lỗi xảy ra, vui lòng thử lại!" });
            }
        }
    }
}
