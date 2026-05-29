using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoHub.Data;
using AutoHub.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace AutoHub.Services
{
    public interface IDashboardService
    {
        Task<decimal> GetTotalRevenueAsync();
        Task<int> GetTotalVehiclesInStockAsync();
        Task<int> GetTotalBrandsAsync();
        Task<int> GetTotalSparePartsAsync();
        Task<List<Order>> GetRecentOrdersAsync(int count = 5);
    }

    public class DashboardService : IDashboardService
    {
        private readonly AppDbContext _context;

        public DashboardService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<decimal> GetTotalRevenueAsync()
        {
            return await _context.Orders
                .Where(o => !o.IsDeleted)
                .SumAsync(o => o.TotalAmount);
        }

        public async Task<int> GetTotalVehiclesInStockAsync()
        {
            var sum = await _context.Vehicles
                .Where(v => !v.IsDeleted)
                .SumAsync(v => (int?)v.Quantity);
            return sum ?? 0;
        }

        public async Task<int> GetTotalBrandsAsync()
        {
            return await _context.Brands
                .Where(b => !b.IsDeleted)
                .CountAsync();
        }

        public async Task<int> GetTotalSparePartsAsync()
        {
            return await _context.SpareParts
                .Where(p => !p.IsDeleted)
                .CountAsync();
        }

        public async Task<List<Order>> GetRecentOrdersAsync(int count = 5)
        {
            return await _context.Orders
                .Where(o => !o.IsDeleted)
                .OrderByDescending(o => o.OrderDate)
                .Take(count)
                .ToListAsync();
        }
    }
}
