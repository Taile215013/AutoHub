using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using AutoHub.Models;
using AutoHub.Models.Entities;
using AutoHub.Repositories;
using AutoHub.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using AutoHub.Services;

namespace AutoHub.Controllers
{
    public class HomeController : Controller
    {
        private readonly IVehicleRepository _vehicleRepository;
        private readonly IServiceRepository _serviceRepository;
        private readonly IUserRepository _userRepository;
        private readonly IAuthService _authService;
        private readonly ILocationService _locationService;
        private readonly ISystemDictionaryService _dictService;
        private readonly IOrderRepository _orderRepository;

        public HomeController(
            IVehicleRepository vehicleRepository, 
            IServiceRepository serviceRepository,
            IUserRepository userRepository,
            IAuthService authService,
            ILocationService locationService,
            ISystemDictionaryService dictService,
            IOrderRepository orderRepository)
        {
            _vehicleRepository = vehicleRepository;
            _serviceRepository = serviceRepository;
            _userRepository = userRepository;
            _authService = authService;
            _locationService = locationService;
            _dictService = dictService;
            _orderRepository = orderRepository;
        }

        public async Task<IActionResult> Index(string? vehicleType, string? bodyStyle, string? fuelType)
        {
            try
            {
                var vehicles = await _vehicleRepository.GetAllWithDetailsAsync(vehicleType, bodyStyle, fuelType);
                var services = await _serviceRepository.GetAllActiveAsync(null);

                var dicts = await _dictService.GetAllAsync();
                ViewBag.VehicleTypes = dicts.Where(d => d.Type == "VehicleType").ToList();
                ViewBag.FuelTypes = dicts.Where(d => d.Type == "FuelType").ToList();

                ViewBag.VehicleType = vehicleType;
                ViewBag.BodyStyle = bodyStyle;
                ViewBag.FuelType = fuelType;
                ViewBag.Services = services;
                ViewBag.Districts = _locationService.GetCities(); // This was GetDistricts keys in mock, mapped to Cities for simplicity.

                return View(vehicles);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DB Error in Index: {ex.Message}");
                ViewBag.VehicleTypes = new List<SystemDictionary>();
                ViewBag.FuelTypes = new List<SystemDictionary>();
                ViewBag.Services = new List<Service>();
                ViewBag.Districts = new List<string>();
                return View(new List<Vehicle>());
            }
        }

        [HttpGet]
        public IActionResult GetCities()
        {
            var cities = _locationService.GetCities();
            return Json(cities);
        }

        [HttpGet]
        public IActionResult GetWards(string district)
        {
            var wards = _locationService.GetWards(district);
            return Json(wards);
        }

        [HttpGet]
        public IActionResult GetDistricts(string city)
        {
            var districts = _locationService.GetDistricts(city);
            return Json(districts);
        }

        // ---------- XÁC THỰC (AUTHENTICATION) ---------- //

        [HttpGet]
        public IActionResult Login()
        {
            if (HttpContext.Session.GetInt32("UserId") != null)
                return RedirectToAction("Account");
                
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
                return RedirectToAction("Account");
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
                return RedirectToAction("Account");

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
                return RedirectToAction("Account");
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
            return RedirectToAction("Index");
        }

        // ---------- TRANG CÁ NHÂN ---------- //

        public async Task<IActionResult> Account()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login");
            }

            try
            {
                var user = await _userRepository.GetByIdAsync(userId.Value);
                if (user == null)
                {
                    HttpContext.Session.Clear();
                    return RedirectToAction("Login");
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
                return RedirectToAction("Login");

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

            return RedirectToAction("Account");
        }

        /// <summary>
        /// Cập nhật thông tin cá nhân của người dùng (họ tên, giới tính, ngày sinh, email, SĐT)
        /// URL: /Home/UpdateProfile (POST - AJAX)
        /// </summary>
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

                // Kiểm tra email đã được dùng bởi tài khoản khác chưa
                if (!string.IsNullOrWhiteSpace(Email) && Email != user.Email)
                {
                    if (await _userRepository.IsEmailTakenAsync(Email, user.Id))
                        return Json(new { success = false, message = "Email này đã được sử dụng bởi tài khoản khác!" });
                }

                // Kiểm tra SĐT đã được dùng bởi tài khoản khác chưa
                if (!string.IsNullOrWhiteSpace(PhoneNumber) && PhoneNumber != user.PhoneNumber)
                {
                    if (await _userRepository.IsPhoneTakenAsync(PhoneNumber, user.Id))
                        return Json(new { success = false, message = "Số điện thoại này đã được sử dụng bởi tài khoản khác!" });
                }

                // Cập nhật thông tin
                user.FirstName = FirstName?.Trim() ?? user.FirstName;
                user.LastName = LastName?.Trim() ?? user.LastName;
                user.Gender = Gender ?? user.Gender;
                user.DateOfBirth = DateOfBirth;
                user.Email = string.IsNullOrWhiteSpace(Email) ? user.Email : Email.Trim();
                user.PhoneNumber = string.IsNullOrWhiteSpace(PhoneNumber) ? user.PhoneNumber : PhoneNumber.Trim();
                user.UpdatedAt = DateTime.UtcNow;

                await _userRepository.UpdateAsync(user);

                // Cập nhật lại Session tên hiển thị
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

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
