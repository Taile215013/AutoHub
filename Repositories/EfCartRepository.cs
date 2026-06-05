using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AutoHub.Data;
using AutoHub.Models.Entities;

namespace AutoHub.Repositories;

public class EfCartRepository : ICartRepository
{
    private readonly AppDbContext _context;

    public EfCartRepository(AppDbContext context) => _context = context;

    public async Task<Cart?> GetByUserIdAsync(int userId)
        => await _context.Carts
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.UserId == userId);

    public async Task<Cart> GetOrCreateAsync(int userId)
    {
        var cart = await GetByUserIdAsync(userId);
        if (cart != null) return cart;

        cart = new Cart { UserId = userId, UpdatedAt = DateTime.UtcNow };
        _context.Carts.Add(cart);
        await _context.SaveChangesAsync();
        return cart;
    }

    public Task SaveAsync() => _context.SaveChangesAsync();

    // CartItem dùng hard delete vì là dữ liệu tạm thời
    public async Task RemoveItemAsync(CartItem item)
    {
        _context.CartItems.Remove(item);
        await _context.SaveChangesAsync();
    }

    public async Task RemoveItemsAsync(IEnumerable<CartItem> items)
    {
        _context.CartItems.RemoveRange(items);
        await _context.SaveChangesAsync();
    }
}
