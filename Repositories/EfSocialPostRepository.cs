using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AutoHub.Data;
using AutoHub.Models.Entities;

namespace AutoHub.Repositories
{
    public class EfSocialPostRepository : ISocialPostRepository
    {
        private readonly AppDbContext _context;

        public EfSocialPostRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<SocialPost>> GetAllPostsWithUsersAsync()
        {
            return await _context.SocialPosts
                .Include(p => p.User)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task AddAsync(SocialPost post)
        {
            await _context.SocialPosts.AddAsync(post);
            await _context.SaveChangesAsync();
        }
    }
}
