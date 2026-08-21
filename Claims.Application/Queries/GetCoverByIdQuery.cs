using Claims.Application.DTO;
using Claims.Application.Persistence;
using Claims.Domain;

namespace Claims.Application.Queries
{
    public class GetCoverByIdQuery
    {
        private readonly ICoverRepository _covers;

        public GetCoverByIdQuery(ICoverRepository covers)
        {
            _covers = covers;
        }

        public async Task<CoverDto?> ExecuteAsync(string id)
        {
            Cover? entity = await _covers.GetByIdAsync(id);

            return entity is null ? null : new CoverDto(entity);
        }
    }
}
