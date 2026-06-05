using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AutoHub.Data;
using AutoHub.Models.Entities;

namespace AutoHub.Repositories;

public class EfUserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public EfUserRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<User>> GetAllAsync()
        => await _context.Users.IgnoreQueryFilters()
            .OrderByDescending(u => u.CreatedAt).ToListAsync();

    public async Task<User?> GetByIdAsync(int id)
        => await _context.Users.IgnoreQueryFilters().FirstOrDefaultAsync(u => u.Id == id);

    public async Task<User?> GetByEmailOrPhoneAsync(string loginInput)
        => await _context.Users
            .FirstOrDefaultAsync(u => u.Username == loginInput || u.Email == loginInput || u.PhoneNumber == loginInput);

    public async Task<bool> IsUsernameTakenAsync(string username, int excludeUserId)
        => await _context.Users.AnyAsync(u => u.Username == username && u.Id != excludeUserId);

    public async Task<bool> IsEmailTakenAsync(string email, int excludeUserId)
        => await _context.Users.AnyAsync(u => u.Email == email && u.Id != excludeUserId);

    public async Task<bool> IsPhoneTakenAsync(string phoneNumber, int excludeUserId)
        => await _context.Users.AnyAsync(u => u.PhoneNumber == phoneNumber && u.Id != excludeUserId);

    public async Task AddAsync(User user)
    {
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user is null) return;
        user.IsDeleted = true;
        user.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
    }
}
