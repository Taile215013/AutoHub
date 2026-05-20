using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AutoHub.Models;
using AutoHub.Models.Entities;
using AutoHub.Repositories;

namespace AutoHub.Controllers
{
    public class HomeController : Controller
    {
        private readonly IVehicleRepository _vehicleRepository;
        private readonly IServiceRepository _serviceRepository;
        private readonly IUserRepository _userRepository;

        private static readonly Dictionary<string, List<string>> DistrictWards2026 = new()
        {
            { "Quận Gò Vấp", new List<string> { "Phường An Hội Tây", "Phường 1", "Phường 3", "Phường 5", "Phường 7" } },
            { "Quận 1", new List<string> { "Phường Bến Nghé", "Phường Bến Thành", "Phường Đa Kao", "Phường Tân Định" } },
            { "Thành phố Thủ Đức", new List<string> { "Phường An Khánh", "Phường Thảo Điền", "Phường Bình Thọ", "Phường Thủ Thiêm" } }
        };

        public HomeController(
            IVehicleRepository vehicleRepository, 
            IServiceRepository serviceRepository,
            IUserRepository userRepository)
        {
            _vehicleRepository = vehicleRepository;
            _serviceRepository = serviceRepository;
            _userRepository = userRepository;
        }

        public async Task<IActionResult> Index(string? vehicleType, string? bodyStyle, string? fuelType)
        {
            var vehicles = await _vehicleRepository.GetAllWithDetailsAsync(vehicleType, bodyStyle, fuelType);
            var services = await _serviceRepository.GetAllActiveAsync(null);

            ViewBag.VehicleType = vehicleType;
            ViewBag.BodyStyle = bodyStyle;
            ViewBag.FuelType = fuelType;
            ViewBag.Services = services;
            ViewBag.Districts = DistrictWards2026.Keys.ToList();

            return View(vehicles);
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

        public async Task<IActionResult> Dashboard(int userId = 1)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                user = new User
                {
                    Id = 1,
                    FirstName = "Tài",
                    LastName = "Nguyễn Thanh",
                    Gender = "Nam",
                    Email = "tai.nguyen@autohub.vn",
                    PhoneNumber = "0912345678",
                    Ward = "Phường An Hội Tây",
                    District = "Quận Gò Vấp",
                    City = "Hồ Chí Minh",
                    RankLevel = "Gold"
                };
            }
            return View(user);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
