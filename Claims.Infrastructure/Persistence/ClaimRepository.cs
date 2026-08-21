using Claims.Application.Persistence;
using Claims.Domain;
using Microsoft.EntityFrameworkCore;

namespace Claims.Infrastructure.Persistence
{
    public class ClaimRepository : IClaimRepository
    {
        private readonly ClaimsContext _context;

        public ClaimRepository(ClaimsContext context)
        {
            _context = context;
        }

        public Task<List<Claim>> GetAllAsync()
        {
            return _context.Claims.ToListAsync();
        }

        public Task<Claim?> GetByIdAsync(string id)
        {
            return _context.Claims.Where(c => c.Id == id).SingleOrDefaultAsync();
        }

        public async Task AddAsync(Claim claim)
        {
            _context.Claims.Add(claim);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveAsync(Claim claim)
        {
            _context.Claims.Remove(claim);
            await _context.SaveChangesAsync();
        }

        public Task<bool> ExistsForCoverAsync(string coverId)
        {
            return _context.Claims.AnyAsync(x => x.CoverId == coverId);
        }
    }
}
