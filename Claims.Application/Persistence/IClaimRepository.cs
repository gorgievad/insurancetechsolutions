using Claims.Domain;

namespace Claims.Application.Persistence
{
    /// <summary>
    /// The claim persistence operations the handlers actually use.
    /// Implemented over the EF Core context in the infrastructure layer.
    /// </summary>
    public interface IClaimRepository
    {
        Task<List<Claim>> GetAllAsync();

        Task<Claim?> GetByIdAsync(string id);

        Task AddAsync(Claim claim);

        Task RemoveAsync(Claim claim);

        /// <summary>
        /// True when at least one claim references the given cover.
        /// </summary>
        Task<bool> ExistsForCoverAsync(string coverId);
    }
}
