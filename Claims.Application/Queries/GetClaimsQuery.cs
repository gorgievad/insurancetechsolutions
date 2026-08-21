using Claims.Application.DTO;
using Claims.Application.Persistence;
using Claims.Domain;

namespace Claims.Application.Queries
{
    public class GetClaimsQuery
    {
        private readonly IClaimRepository _claims;

        public GetClaimsQuery(IClaimRepository claims)
        {
            _claims = claims;
        }

        public async Task<List<ClaimDto>> ExecuteAsync()
        {
            List<Claim> entities = await _claims.GetAllAsync();

            return entities.Select(e => new ClaimDto(e)).ToList();
        }
    }
}
