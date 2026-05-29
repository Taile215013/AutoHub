using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AutoHub.Data;
using AutoHub.Models.Entities;

namespace AutoHub.Repositories;

public class EfOrderRepository : IOrderRepository
{
    private readonly AppDbContext _context;

    public EfOrderRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Order order)
    {
        await _context.Orders.AddAsync(order);
        await _context.SaveChangesAsync();
    }

    public async Task<Order?> GetByIdWithDetailsAsync(int id)
    {
        return await _context.Orders
            .Include(o => o.User)
            .Include(o => o.OrderDetails)
            .FirstOrDefaultAsync(o => o.Id == id);
    }

    public async Task<System.Collections.Generic.List<Order>> GetOrdersByUserIdAsync(int userId)
    {
        return await _context.Orders
            .Where(o => o.UserId == userId && !o.IsDeleted)
            .Include(o => o.OrderDetails)
            .ToListAsync();
    }
}
