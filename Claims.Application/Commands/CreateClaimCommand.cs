using Claims.Application.Auditing;
using Claims.Application.DTO;
using Claims.Application.Persistence;
using Claims.Domain;
using Claims.Domain.Common;

namespace Claims.Application.Commands
{
    public record CreateClaimCommand(CreateClaimRequest Request);

    public class CreateClaimCommandHandler
    {
        private readonly IClaimRepository _claims;
        private readonly ICoverRepository _covers;
        private readonly IAuditQueue _auditQueue;

        public CreateClaimCommandHandler(IClaimRepository claims, ICoverRepository covers, IAuditQueue auditQueue)
        {
            _claims = claims;
            _covers = covers;
            _auditQueue = auditQueue;
        }

        /// <summary>
        /// Creates a claim if the domain validations pass.
        /// Returns a failure result when validation fails.
        /// </summary>
        public async Task<Result<ClaimDto>> HandleAsync(CreateClaimCommand command)
        {
            CreateClaimRequest request = command.Request;

            string? coverId = request.CoverId;

            if (string.IsNullOrEmpty(coverId))
            {
                return Result<ClaimDto>.Failure(ResultErrorCodes.CoverIdRequired);
            }

            Cover? cover = await _covers.GetByIdAsync(coverId);

            Result<Claim> createResult = Claim.Create(coverId: coverId,
                                                      created: request.Created,
                                                      name: request.Name ?? string.Empty,
                                                      type: request.Type,
                                                      damageCost: request.DamageCost,
                                                      relatedCover: cover);

            if (!createResult.IsSuccess || createResult.Value is null)
            {
                return Result<ClaimDto>.Failure(createResult.Error);
            }

            Claim claim = createResult.Value;

            await _claims.AddAsync(claim);

            await _auditQueue.EnqueueAsync(new AuditMessage
            {
                EntityType = AuditEntityType.Claim,
                EntityId = claim.Id,
                HttpRequestType = "POST",
                Created = DateTime.UtcNow
            });

            return Result<ClaimDto>.Success(new ClaimDto(claim));
        }
    }
}
