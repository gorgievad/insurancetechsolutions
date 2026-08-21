using Claims.Application.Auditing;
using Claims.Application.DTO;
using Claims.Application.Persistence;
using Claims.Application.Services;
using Claims.Domain;
using Claims.Domain.Common;

namespace Claims.Application.Commands
{
    public record CreateCoverCommand(CreateCoverRequest Request);

    public class CreateCoverCommandHandler
    {
        private readonly ICoverRepository _covers;
        private readonly IAuditQueue _auditQueue;
        private readonly ComputePremiumService _computePremiumService;

        public CreateCoverCommandHandler(ICoverRepository covers, IAuditQueue auditQueue, ComputePremiumService computePremiumService)
        {
            _covers = covers;
            _auditQueue = auditQueue;
            _computePremiumService = computePremiumService;
        }

        /// <summary>
        /// Creates a cover if the domain validations pass.
        /// Returns a failure result when validation fails.
        /// </summary>
        public async Task<Result<CoverDto>> HandleAsync(CreateCoverCommand command)
        {
            CreateCoverRequest request = command.Request;

            Result<Cover> createResult = Cover.Create(startDate: request.StartDate,
                                                      endDate: request.EndDate,
                                                      type: request.Type);

            if (!createResult.IsSuccess || createResult.Value is null)
            {
                return Result<CoverDto>.Failure(createResult.Error);
            }

            Cover cover = createResult.Value;

            decimal premium = _computePremiumService.ComputePremium(cover.StartDate, cover.EndDate, cover.Type);

            cover.SetPremium(premium);

            await _covers.AddAsync(cover);

            await _auditQueue.EnqueueAsync(new AuditMessage
            {
                EntityType = AuditEntityType.Cover,
                EntityId = cover.Id,
                HttpRequestType = "POST",
                Created = DateTime.UtcNow
            });

            return Result<CoverDto>.Success(new CoverDto(cover));
        }
    }
}
