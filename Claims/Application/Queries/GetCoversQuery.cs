namespace Claims.Application.Queries
{
    using Microsoft.EntityFrameworkCore;
    using Claims.Domain;
    using Claims.Infrastructure;

    public class GetCoversQuery
    {
        private readonly ClaimsContext _context;

        public GetCoversQuery(ClaimsContext context)
        {
            _context = context;
        }

        public async Task<List<Claims.Domain.DTO.CoverDto>> ExecuteAsync()
        {
            List<Cover> entities = await _context.Covers.ToListAsync();

            return entities.Select(e => new Claims.Domain.DTO.CoverDto(e)).ToList();
        }
    }
}
