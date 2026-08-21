using Claims.Application.Persistence;
using Claims.Domain;
using Microsoft.EntityFrameworkCore;

namespace Claims.Infrastructure.Persistence
{
    public class CoverRepository : ICoverRepository
    {
        private readonly ClaimsContext _context;

        public CoverRepository(ClaimsContext context)
        {
            _context = context;
        }

        public Task<List<Cover>> GetAllAsync()
        {
            return _context.Covers.ToListAsync();
        }

        public Task<Cover?> GetByIdAsync(string id)
        {
            return _context.Covers.Where(c => c.Id == id).SingleOrDefaultAsync();
        }

        public async Task AddAsync(Cover cover)
        {
            _context.Covers.Add(cover);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveAsync(Cover cover)
        {
            _context.Covers.Remove(cover);
            await _context.SaveChangesAsync();
        }
    }
}
