using Claims.Domain;

namespace Claims.Application.Persistence
{
    /// <summary>
    /// The cover persistence operations the handlers actually use.
    /// Implemented over the EF Core context in the infrastructure layer.
    /// </summary>
    public interface ICoverRepository
    {
        Task<List<Cover>> GetAllAsync();

        Task<Cover?> GetByIdAsync(string id);

        Task AddAsync(Cover cover);

        Task RemoveAsync(Cover cover);
    }
}
