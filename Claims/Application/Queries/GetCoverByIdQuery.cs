using Microsoft.EntityFrameworkCore;
using Claims.Domain;
using Claims.Infrastructure;

namespace Claims.Application.Queries
{
    public class GetCoverByIdQuery
    {
        private readonly ClaimsContext _context;

        public GetCoverByIdQuery(ClaimsContext context)
        {
            _context = context;
        }

        public async Task<Claims.Domain.DTO.CoverDto?> ExecuteAsync(string id)
        {
            Cover? entity = await _context.Covers.Where(c => c.Id == id).SingleOrDefaultAsync();

            return entity is null ? null : new Claims.Domain.DTO.CoverDto(entity);
        }
    }
}
