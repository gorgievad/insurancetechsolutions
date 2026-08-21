using Claims.Application.Commands;
using Claims.Application.Common;
using Claims.Application.Queries;
using Claims.Controllers.Common;
using Claims.Domain;
using Claims.Domain.DTO;
using Microsoft.AspNetCore.Mvc;

namespace Claims.API.Controllers;

/// <summary>
/// Controller for operations relating to COVER entity.
/// </summary>
[ApiController]
[Route("[controller]")]
public class CoversController : ControllerBase
{
    private readonly CreateCoverCommandHandler _createCoverHandler;
    private readonly DeleteCoverCommandHandler _deleteCoverHandler;
    private readonly GetCoversQuery _getCoversQuery;
    private readonly GetCoverByIdQuery _getCoverByIdQuery;
    private readonly ComputePremiumQuery _computePremiumQuery;

    public CoversController(CreateCoverCommandHandler createCoverHandler,
                            DeleteCoverCommandHandler deleteCoverHandler, 
                            GetCoversQuery getCoversQuery, 
                            GetCoverByIdQuery getCoverByIdQuery, 
                            ComputePremiumQuery computePremiumQuery)
    {
        _createCoverHandler = createCoverHandler;
        _deleteCoverHandler = deleteCoverHandler;
        _getCoversQuery = getCoversQuery;
        _getCoverByIdQuery = getCoverByIdQuery;
        _computePremiumQuery = computePremiumQuery;
    }

    /// <summary>
    /// GET endpoint for computing the premium based on start date, end date, and cover type. Returns the computed premium as a decimal value.
    /// Nothing is created or stored, so this is a read-only operation.
    /// </summary>
    /// <param name="startDate"></param>
    /// <param name="endDate"></param>
    /// <param name="coverType"></param>
    /// <returns>ActionResult&lt;decimal&gt;</returns>
    [HttpGet("compute")]
    public ActionResult<decimal> ComputePremium(DateTime startDate, DateTime endDate, CoverType coverType)
    {
        Result<decimal> result = _computePremiumQuery.Execute(startDate, endDate, coverType);

        return result.ToActionResult();
    }

    /// <summary>
    /// GET endpoint for retrieving all covers. Returns a list of cover data.
    /// </summary>
    /// <returns>ActionResult&lt;IEnumerable&lt;Claims.Domain.DTO.CoverDto&gt;&gt;</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Claims.Domain.DTO.CoverDto>>> GetAllCoversAsync()
    {
        List<CoverDto> results = await _getCoversQuery.ExecuteAsync();
        return Ok(results);
    }

    /// <summary>
    /// GET endpoint for retrieving a cover by its ID. Returns the cover data if found.
    /// </summary>
    /// <param name="id"></param>
    /// <returns>ActionResult&lt;CoverDto&gt;</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<CoverDto>> GetCoverByIdAsync(string id)
    {
        CoverDto? cover = await _getCoverByIdQuery.ExecuteAsync(id);

        return cover is null ? NotFound() : Ok(cover);
    }

    /// <summary>
    /// POST endpoint for creating a new cover. Returns the created cover data.
    /// </summary>
    /// <param name="request"></param>
    /// <returns>ActionResult&lt;CoverDto&gt;</returns>
    [HttpPost]
    public async Task<ActionResult<CoverDto>> CreateCoverAsync([FromBody] CreateCoverRequest request)
    {
        Result<CoverDto> result = await _createCoverHandler.HandleAsync(new CreateCoverCommand(request));

        return result.ToActionResult();
    }

    /// <summary>
    /// DELETE endpoint for deleting a cover by its ID. Returns no content on successful deletion.
    /// </summary>
    /// <param name="id"></param>
    /// <returns>IActionResult</returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync(string id)
    {
        Result result = await _deleteCoverHandler.HandleAsync(new DeleteCoverCommand(id));

        return result.ToNoContentResult();
    }
}
