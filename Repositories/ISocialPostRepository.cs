using System.Collections.Generic;
using System.Threading.Tasks;
using AutoHub.Models.Entities;

namespace AutoHub.Repositories
{
    public interface ISocialPostRepository
    {
        Task<IEnumerable<SocialPost>> GetAllPostsWithUsersAsync();
        Task AddAsync(SocialPost post);
    }
}
