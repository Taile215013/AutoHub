using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using AutoHub.Models.Entities;
using AutoHub.Repositories;

namespace AutoHub.Controllers;

/// <summary>
/// Xử lý giỏ hàng của người dùng đã đăng nhập.
/// Người dùng chưa đăng nhập sẽ dùng localStorage ở phía client (cart.js).
/// </summary>
public class CartController : Controller
{
    private readonly ICartRepository _cartRepository;

    public CartController(ICartRepository cartRepository)
        => _cartRepository = cartRepository;

    // ── Helpers ────────────────────────────────────────────────────────────

    private int? CurrentUserId
        => HttpContext.Session.GetInt32("UserId");

    private IActionResult NotLoggedIn()
        => Json(new { success = false, message = "Chưa đăng nhập" });

    // ── Endpoints ──────────────────────────────────────────────────────────

    [HttpGet]
    public async Task<IActionResult> GetCart()
    {
        if (CurrentUserId is not int userId) return NotLoggedIn();

        var cart = await _cartRepository.GetByUserIdAsync(userId);
        if (cart == null) return Json(Array.Empty<object>());

        var items = cart.Items.Select(i => new
        {
            id   = i.Id,
            name = i.Name,
            price = i.Price,
            type = i.ProductType,
            qty  = i.Quantity
        });

        return Json(items);
    }

    [HttpPost]
    public async Task<IActionResult> AddToCart([FromBody] CartItemInput input)
    {
        if (CurrentUserId is not int userId) return NotLoggedIn();

        var cart = await _cartRepository.GetOrCreateAsync(userId);

        var existing = cart.Items.FirstOrDefault(i => i.Name == input.Name);
        if (existing != null)
        {
            existing.Quantity += Math.Max(input.Qty, 1);
            existing.UpdatedAt = DateTime.UtcNow;
        }
        else
        {
            cart.Items.Add(new CartItem
            {
                Name        = input.Name,
                Price       = input.Price,
                ProductType = input.Type,
                Quantity    = Math.Max(input.Qty, 1),
                UpdatedAt   = DateTime.UtcNow
            });
        }

        cart.UpdatedAt = DateTime.UtcNow;
        await _cartRepository.SaveAsync();

        return Json(new { success = true });
    }

    [HttpPost]
    public async Task<IActionResult> RemoveItem([FromBody] CartItemInput input)
    {
        if (CurrentUserId is not int userId) return NotLoggedIn();

        var cart = await _cartRepository.GetByUserIdAsync(userId);
        var item = cart?.Items.FirstOrDefault(i => i.Name == input.Name);
        if (item != null) await _cartRepository.RemoveItemAsync(item);

        return Json(new { success = true });
    }

    [HttpPost]
    public async Task<IActionResult> UpdateQuantity([FromBody] CartItemInput input)
    {
        if (CurrentUserId is not int userId) return NotLoggedIn();

        var cart = await _cartRepository.GetByUserIdAsync(userId);
        var item = cart?.Items.FirstOrDefault(i => i.Name == input.Name);
        if (item != null)
        {
            item.Quantity  = input.Qty;
            item.UpdatedAt = DateTime.UtcNow;
            cart!.UpdatedAt = DateTime.UtcNow;
            await _cartRepository.SaveAsync();
        }

        return Json(new { success = true });
    }

    [HttpPost]
    public async Task<IActionResult> ClearCart()
    {
        if (CurrentUserId is not int userId) return NotLoggedIn();

        var cart = await _cartRepository.GetByUserIdAsync(userId);
        if (cart?.Items.Any() == true)
            await _cartRepository.RemoveItemsAsync(cart.Items.ToList());

        return Json(new { success = true });
    }

    [HttpPost]
    public async Task<IActionResult> SyncCart([FromBody] List<CartItemInput>? localItems)
    {
        if (CurrentUserId is not int userId) return NotLoggedIn();
        if (localItems is null || !localItems.Any()) return Json(new { success = true });

        var cart = await _cartRepository.GetOrCreateAsync(userId);

        foreach (var input in localItems)
        {
            var existing = cart.Items.FirstOrDefault(i => i.Name == input.Name);
            if (existing != null)
            {
                existing.Quantity += Math.Max(input.Qty, 1);
                existing.UpdatedAt = DateTime.UtcNow;
            }
            else
            {
                cart.Items.Add(new CartItem
                {
                    Name        = input.Name,
                    Price       = input.Price,
                    ProductType = input.Type,
                    Quantity    = Math.Max(input.Qty, 1),
                    UpdatedAt   = DateTime.UtcNow
                });
            }
        }

        cart.UpdatedAt = DateTime.UtcNow;
        await _cartRepository.SaveAsync();

        return Json(new { success = true });
    }
}

/// <summary>DTO từ client gửi lên — tách ra để không bind trực tiếp vào entity.</summary>
public sealed record CartItemInput(string Name, decimal Price, string Type, int Qty);
