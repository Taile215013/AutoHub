using Microsoft.AspNetCore.Mvc;
using AutoHub.Repositories;
using AutoHub.Models.Entities;
using System.Threading.Tasks;
using System;

namespace AutoHub.Controllers
{
    public class VehicleController : Controller
    {
        private readonly IVehicleRepository _vehicleRepository;

        public VehicleController(IVehicleRepository vehicleRepository)
        {
            _vehicleRepository = vehicleRepository;
        }

        // Route cho category (ví dụ: /auto/toyota-camry-2023-12.html)
        [Route("{category}/{slug}-{id}.html", Name = "VehicleDetail")]
        public async Task<IActionResult> Detail(string category, string slug, int id)
        {
            try
            {
                // Sử dụng ID để tìm sản phẩm đảm bảo độ chính xác 100%
                var vehicle = await _vehicleRepository.GetByIdWithDetailsAsync(id);
                
                if (vehicle == null)
                {
                    return NotFound();
                }

                // Nếu có repository để lấy chi tiết hơn (như load kèm Brand, Colors, v.v.),
                // có thể thay thế bằng hàm GetVehicleDetailsByIdAsync.

                return View(vehicle);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading vehicle details: {ex.Message}");
                return View("Error");
            }
        }
        
        // Route cho danh mục con (ví dụ: /motorcycle/tay-ga/honda-sh-15.html)
        [Route("{category}/{subcategory}/{slug}-{id}.html", Name = "VehicleDetailSub")]
        public async Task<IActionResult> DetailSub(string category, string subcategory, string slug, int id)
        {
            return await Detail(category, slug, id);
        }
    }
}
