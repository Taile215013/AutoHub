using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AutoHub.Services;

namespace AutoHub.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DashboardController : Controller
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            ViewBag.TotalRevenue = await _dashboardService.GetTotalRevenueAsync();
            ViewBag.TotalVehiclesInStock = await _dashboardService.GetTotalVehiclesInStockAsync();
            ViewBag.TotalBrands = await _dashboardService.GetTotalBrandsAsync();
            ViewBag.TotalSpareParts = await _dashboardService.GetTotalSparePartsAsync();
            ViewBag.RecentOrders = await _dashboardService.GetRecentOrdersAsync(5);

            return View();
        }
    }
}
