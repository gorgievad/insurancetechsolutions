namespace Claims.Application.Queries
{
    using Microsoft.EntityFrameworkCore;
    using Claims.Domain;
    using Claims.Infrastructure;

    public class GetClaimsQuery
    {
        private readonly ClaimsContext _context;

        public GetClaimsQuery(ClaimsContext context)
        {
            _context = context;
        }

        public async Task<List<Claims.Domain.DTO.ClaimDto>> ExecuteAsync()
        {
            List<Claim> entities = await _context.Claims.ToListAsync();

            return entities.Select(e => new Claims.Domain.DTO.ClaimDto(e)).ToList();
        }
    }
}
