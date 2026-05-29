using System.Threading.Tasks;
using AutoHub.Models.Entities;

namespace AutoHub.Repositories;

public interface IOrderRepository
{
    Task AddAsync(Order order);

    Task<Order?> GetByIdWithDetailsAsync(int id);
    Task<System.Collections.Generic.List<Order>> GetOrdersByUserIdAsync(int userId);
}
