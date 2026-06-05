using System.Collections.Generic;
using System.Threading.Tasks;
using AutoHub.Models.Entities;

namespace AutoHub.Repositories;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(int id);
    Task<IEnumerable<User>> GetAllAsync();
    Task<User?> GetByEmailOrPhoneAsync(string loginInput);
    Task<bool> IsUsernameTakenAsync(string username, int excludeUserId);
    Task<bool> IsEmailTakenAsync(string email, int excludeUserId);
    Task<bool> IsPhoneTakenAsync(string phoneNumber, int excludeUserId);
    Task AddAsync(User user);
    Task UpdateAsync(User user);
    Task DeleteAsync(int id);
}
