using Claims.Application.DTO;
using Claims.Application.Persistence;
using Claims.Domain;

namespace Claims.Application.Queries
{
    public class GetCoversQuery
    {
        private readonly ICoverRepository _covers;

        public GetCoversQuery(ICoverRepository covers)
        {
            _covers = covers;
        }

        public async Task<List<CoverDto>> ExecuteAsync()
        {
            List<Cover> entities = await _covers.GetAllAsync();

            return entities.Select(e => new CoverDto(e)).ToList();
        }
    }
}
