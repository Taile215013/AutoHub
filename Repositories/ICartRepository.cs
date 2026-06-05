using System.Collections.Generic;
using System.Threading.Tasks;
using AutoHub.Models.Entities;

namespace AutoHub.Repositories;

/// <summary>
/// Quản lý giỏ hàng của người dùng đã đăng nhập.
/// CartItem dùng hard delete vì đây là dữ liệu tạm thời, không cần audit trail.
/// </summary>
public interface ICartRepository
{
    Task<Cart?> GetByUserIdAsync(int userId);
    Task<Cart> GetOrCreateAsync(int userId);
    Task SaveAsync();
    Task RemoveItemAsync(CartItem item);
    Task RemoveItemsAsync(IEnumerable<CartItem> items);
}
