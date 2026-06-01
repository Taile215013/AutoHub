using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using AutoHub.Data;
using AutoHub.Models.Entities;
using System.Collections.Generic;

namespace AutoHub.Controllers
{
    public class CartController : Controller
    {
        private readonly AppDbContext _context;

        public CartController(AppDbContext context)
        {
            _context = context;
        }

        private int? GetUserId()
        {
            return HttpContext.Session.GetInt32("UserId");
        }

        [HttpGet]
        public async Task<IActionResult> GetCart()
        {
            var userId = GetUserId();
            if (userId == null) return Json(new { success = false, message = "Not logged in" });

            var cart = await _context.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                return Json(new List<object>()); // empty cart
            }

            var items = cart.Items.Select(i => new
            {
                id = i.Id,
                name = i.Name,
                price = i.Price,
                type = i.ProductType,
                qty = i.Quantity
            });

            return Json(items);
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart([FromBody] CartItemInput input)
        {
            var userId = GetUserId();
            if (userId == null) return Json(new { success = false });

            var cart = await _context.Carts.Include(c => c.Items).FirstOrDefaultAsync(c => c.UserId == userId);
            if (cart == null)
            {
                cart = new Cart { UserId = userId.Value, UpdatedAt = DateTime.UtcNow };
                _context.Carts.Add(cart);
                await _context.SaveChangesAsync();
            }

            var existingItem = cart.Items.FirstOrDefault(i => i.Name == input.Name);
            if (existingItem != null)
            {
                existingItem.Quantity += (input.Qty > 0 ? input.Qty : 1);
                existingItem.UpdatedAt = DateTime.UtcNow;
            }
            else
            {
                cart.Items.Add(new CartItem
                {
                    Name = input.Name,
                    Price = input.Price,
                    ProductType = input.Type,
                    Quantity = input.Qty > 0 ? input.Qty : 1,
                    UpdatedAt = DateTime.UtcNow
                });
            }

            cart.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> RemoveItem([FromBody] CartItemInput input)
        {
            var userId = GetUserId();
            if (userId == null) return Json(new { success = false });

            var cart = await _context.Carts.Include(c => c.Items).FirstOrDefaultAsync(c => c.UserId == userId);
            if (cart != null)
            {
                var item = cart.Items.FirstOrDefault(i => i.Name == input.Name);
                if (item != null)
                {
                    _context.CartItems.Remove(item);
                    cart.UpdatedAt = DateTime.UtcNow;
                    await _context.SaveChangesAsync();
                }
            }
            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateQuantity([FromBody] CartItemInput input)
        {
            var userId = GetUserId();
            if (userId == null) return Json(new { success = false });

            var cart = await _context.Carts.Include(c => c.Items).FirstOrDefaultAsync(c => c.UserId == userId);
            if (cart != null)
            {
                var item = cart.Items.FirstOrDefault(i => i.Name == input.Name);
                if (item != null)
                {
                    item.Quantity = input.Qty;
                    item.UpdatedAt = DateTime.UtcNow;
                    cart.UpdatedAt = DateTime.UtcNow;
                    await _context.SaveChangesAsync();
                }
            }
            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> ClearCart()
        {
            var userId = GetUserId();
            if (userId == null) return Json(new { success = false });

            var cart = await _context.Carts.Include(c => c.Items).FirstOrDefaultAsync(c => c.UserId == userId);
            if (cart != null)
            {
                _context.CartItems.RemoveRange(cart.Items);
                cart.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> SyncCart([FromBody] List<CartItemInput> localItems)
        {
            var userId = GetUserId();
            if (userId == null) return Json(new { success = false });
            if (localItems == null || !localItems.Any()) return Json(new { success = true });

            var cart = await _context.Carts.Include(c => c.Items).FirstOrDefaultAsync(c => c.UserId == userId);
            if (cart == null)
            {
                cart = new Cart { UserId = userId.Value, UpdatedAt = DateTime.UtcNow };
                _context.Carts.Add(cart);
                await _context.SaveChangesAsync(); // save to get ID
            }

            foreach (var input in localItems)
            {
                var existingItem = cart.Items.FirstOrDefault(i => i.Name == input.Name);
                if (existingItem != null)
                {
                    existingItem.Quantity += (input.Qty > 0 ? input.Qty : 1);
                    existingItem.UpdatedAt = DateTime.UtcNow;
                }
                else
                {
                    cart.Items.Add(new CartItem
                    {
                        Name = input.Name,
                        Price = input.Price,
                        ProductType = input.Type,
                        Quantity = input.Qty > 0 ? input.Qty : 1,
                        UpdatedAt = DateTime.UtcNow
                    });
                }
            }

            cart.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }
    }

    public class CartItemInput
    {
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Type { get; set; } = string.Empty;
        public int Qty { get; set; }
    }
}
