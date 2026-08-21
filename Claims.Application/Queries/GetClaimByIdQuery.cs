using Claims.Application.DTO;
using Claims.Application.Persistence;
using Claims.Domain;

namespace Claims.Application.Queries
{
    public class GetClaimByIdQuery
    {
        private readonly IClaimRepository _claims;

        public GetClaimByIdQuery(IClaimRepository claims)
        {
            _claims = claims;
        }

        public async Task<ClaimDto?> ExecuteAsync(string id)
        {
            Claim? entity = await _claims.GetByIdAsync(id);

            return entity is null ? null : new ClaimDto(entity);
        }
    }
}
