namespace Claims.Application.Queries
{
    using Microsoft.EntityFrameworkCore;
    using Claims.Domain;
    using Claims.Infrastructure;

    public class GetClaimByIdQuery
    {
        private readonly ClaimsContext _context;

        public GetClaimByIdQuery(ClaimsContext context)
        {
            _context = context;
        }

        public async Task<Claims.Domain.DTO.ClaimDto?> ExecuteAsync(string id)
        {
            Claim? entity = await _context.Claims.Where(c => c.Id == id).SingleOrDefaultAsync();

            return entity is null ? null : new Claims.Domain.DTO.ClaimDto(entity);
        }
    }
}
