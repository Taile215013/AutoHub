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

    public async Task<User?> GetByIdAsync(int id)
    {
        return await _context.Users.FindAsync(id);
    }

    public async Task<User?> GetByEmailOrPhoneAsync(string loginInput)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Email == loginInput || u.PhoneNumber == loginInput);
    }

    public async Task<bool> IsEmailTakenAsync(string email, int excludeUserId)
    {
        return await _context.Users
            .AnyAsync(u => u.Email == email && u.Id != excludeUserId);
    }

    public async Task<bool> IsPhoneTakenAsync(string phoneNumber, int excludeUserId)
    {
        return await _context.Users
            .AnyAsync(u => u.PhoneNumber == phoneNumber && u.Id != excludeUserId);
    }

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
}
