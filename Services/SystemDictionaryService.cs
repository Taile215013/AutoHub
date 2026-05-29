using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoHub.Data;
using AutoHub.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace AutoHub.Services
{
    public interface ISystemDictionaryService
    {
        Task<List<SystemDictionary>> GetDictionariesByTypeAsync(string type);
        Task<List<SystemDictionary>> GetAllAsync();
    }

    public class SystemDictionaryService : ISystemDictionaryService
    {
        private readonly AppDbContext _context;

        public SystemDictionaryService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<SystemDictionary>> GetDictionariesByTypeAsync(string type)
        {
            return await _context.SystemDictionaries
                .Where(d => d.Type == type)
                .ToListAsync();
        }

        public async Task<List<SystemDictionary>> GetAllAsync()
        {
            return await _context.SystemDictionaries.ToListAsync();
        }
    }
}
