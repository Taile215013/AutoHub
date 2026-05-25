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

namespace AutoHub.Controllers
{
    public class HomeController : Controller
    {
        private readonly IVehicleRepository _vehicleRepository;
        private readonly IServiceRepository _serviceRepository;
        private readonly IUserRepository _userRepository;
        private readonly AppDbContext _context;

        private static readonly Dictionary<string, List<string>> DistrictWards2026 = new()
        {
            { "Quận Gò Vấp", new List<string> { "Phường An Hội Tây", "Phường 1", "Phường 3", "Phường 5", "Phường 7" } },
            { "Quận 1", new List<string> { "Phường Bến Nghé", "Phường Bến Thành", "Phường Đa Kao", "Phường Tân Định" } },
            { "Thành phố Thủ Đức", new List<string> { "Phường An Khánh", "Phường Thảo Điền", "Phường Bình Thọ", "Phường Thủ Thiêm" } }
        };

        public HomeController(
            IVehicleRepository vehicleRepository, 
            IServiceRepository serviceRepository,
            IUserRepository userRepository,
            AppDbContext context)
        {
            _vehicleRepository = vehicleRepository;
            _serviceRepository = serviceRepository;
            _userRepository = userRepository;
            _context = context;
        }

        public async Task<IActionResult> Index(string? vehicleType, string? bodyStyle, string? fuelType)
        {
            try
            {
                var vehicles = await _vehicleRepository.GetAllWithDetailsAsync(vehicleType, bodyStyle, fuelType);
                var services = await _serviceRepository.GetAllActiveAsync(null);

                var dicts = await _context.SystemDictionaries.ToListAsync();
                ViewBag.VehicleTypes = dicts.Where(d => d.Type == "VehicleType").ToList();
                ViewBag.FuelTypes = dicts.Where(d => d.Type == "FuelType").ToList();

                ViewBag.VehicleType = vehicleType;
                ViewBag.BodyStyle = bodyStyle;
                ViewBag.FuelType = fuelType;
                ViewBag.Services = services;
                ViewBag.Districts = DistrictWards2026.Keys.ToList();

                return View(vehicles);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DB Error in Index: {ex.Message}");
                ViewBag.VehicleTypes = new List<SystemDictionary>();
                ViewBag.FuelTypes = new List<SystemDictionary>();
                ViewBag.Services = new List<Service>();
                ViewBag.Districts = DistrictWards2026.Keys.ToList();
                return View(new List<Vehicle>());
            }
        }

        [HttpGet]
        public IActionResult GetWards(string district)
        {
            if (DistrictWards2026.TryGetValue(district, out var wards))
            {
                return Json(wards);
            }
            return Json(new List<string>());
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
                var user = await _userRepository.GetByEmailOrPhoneAsync(loginInput);
                if (user == null)
                {
                    ViewBag.Error = "Tài khoản không tồn tại!";
                    return View();
                }

                if (user.PasswordHash != HashPassword(password))
                {
                    ViewBag.Error = "Mật khẩu không chính xác!";
                    return View();
                }

                HttpContext.Session.SetInt32("UserId", user.Id);
                HttpContext.Session.SetString("UserName", $"{user.LastName} {user.FirstName}");
                
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
                if (Password != ConfirmPassword)
                {
                    ViewBag.Error = "Mật khẩu xác nhận không khớp!";
                    return View(user);
                }

                if (string.IsNullOrWhiteSpace(user.Email) && string.IsNullOrWhiteSpace(user.PhoneNumber))
                {
                    ViewBag.Error = "Vui lòng nhập Email hoặc Số điện thoại!";
                    return View(user);
                }

                if (!string.IsNullOrWhiteSpace(user.Email) && await _userRepository.IsEmailTakenAsync(user.Email, 0))
                {
                    ViewBag.Error = "Email này đã được sử dụng!";
                    return View(user);
                }

                if (!string.IsNullOrWhiteSpace(user.PhoneNumber) && await _userRepository.IsPhoneTakenAsync(user.PhoneNumber, 0))
                {
                    ViewBag.Error = "Số điện thoại này đã được sử dụng!";
                    return View(user);
                }

                user.PasswordHash = HashPassword(Password);
                user.StreetName = user.StreetName ?? "Chưa cập nhật";
                user.Ward = user.Ward ?? "Chưa cập nhật";
                user.District = user.District ?? "Chưa cập nhật";
                user.City = "Hồ Chí Minh";
                
                await _userRepository.AddAsync(user);

                HttpContext.Session.SetInt32("UserId", user.Id);
                HttpContext.Session.SetString("UserName", $"{user.LastName} {user.FirstName}");

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

                var orders = await _context.Orders
                    .Where(o => o.UserId == userId && !o.IsDeleted)
                    .Include(o => o.OrderDetails)
                    .ToListAsync();

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

        private static string HashPassword(string password)
        {
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
            return Convert.ToHexStringLower(bytes);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
