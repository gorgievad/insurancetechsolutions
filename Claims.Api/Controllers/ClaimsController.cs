using Claims.Api.Common;
using Claims.Application.Commands;
using Claims.Application.DTO;
using Claims.Application.Queries;
using Claims.Domain.Common;
using Microsoft.AspNetCore.Mvc;
namespace Claims.Api.Controllers
{
    /// <summary>
    /// Controller for operations for entity type CLAIM.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class ClaimsController : ControllerBase
    {
        private readonly GetClaimsQuery _getClaimsQuery;
        private readonly CreateClaimCommandHandler _createClaimHandler;
        private readonly GetClaimByIdQuery _getClaimByIdQuery;
        private readonly DeleteClaimCommandHandler _deleteClaimHandler;

        public ClaimsController(GetClaimsQuery getClaimsQuery,
                                CreateClaimCommandHandler createClaimHandler,
                                GetClaimByIdQuery getClaimByIdQuery,
                                DeleteClaimCommandHandler deleteClaimHandler)
        {
            _getClaimsQuery = getClaimsQuery;
            _createClaimHandler = createClaimHandler;
            _getClaimByIdQuery = getClaimByIdQuery;
            _deleteClaimHandler = deleteClaimHandler;
        }

        /// <summary>
        /// GET endpoint for all claims that returns all data
        /// </summary>
        /// <returns>IEnumerable&lt;ClaimDto&gt;</returns>
        [HttpGet]
        public async Task<IEnumerable<ClaimDto>> GetAllClaimsAsync()
        {
            return await _getClaimsQuery.ExecuteAsync();
        }

        /// <summary>
        /// POST endpoint for creating a new claim. Returns the created claim data.
        /// </summary>
        /// <param name="request"></param>
        /// <returns>ActionResult&lt;ClaimDto&gt;</returns>
        [HttpPost]
        public async Task<ActionResult<ClaimDto>> CreateClaimAsync([FromBody] CreateClaimRequest request)
        {
            Result<ClaimDto> result = await _createClaimHandler.HandleAsync(new CreateClaimCommand(request));

            return result.ToActionResult();
        }

        /// <summary>
        /// DELETE endpoint for deleting a claim by its ID if in the system. Returns no content.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>IActionResult</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClaimAsync(string id)
        {
            Result result = await _deleteClaimHandler.HandleAsync(new DeleteClaimCommand(id));

            return result.ToNoContentResult();
        }

        /// <summary>
        /// GET endpoint for retrieving a claim by its ID if in the system. Returns the claim data.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>ActionResult&lt;ClaimDto&gt;</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<ClaimDto>> GetClaimByIdAsync(string id)
        {
            ClaimDto? claim = await _getClaimByIdQuery.ExecuteAsync(id);

            return claim is null ? NotFound() : Ok(claim);
        }
    }

}
