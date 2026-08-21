using Claims.Application.Auditing;
using Claims.Application.Persistence;
using Claims.Domain;
using Claims.Domain.Common;

namespace Claims.Application.Commands
{
    public record DeleteCoverCommand(string Id);

    public class DeleteCoverCommandHandler
    {
        private readonly ICoverRepository _covers;
        private readonly IClaimRepository _claims;
        private readonly IAuditQueue _auditQueue;

        public DeleteCoverCommandHandler(ICoverRepository covers, IClaimRepository claims, IAuditQueue auditQueue)
        {
            _covers = covers;
            _claims = claims;
            _auditQueue = auditQueue;
        }

        /// <summary>
        /// Deletes the cover with the given id.
        /// Returns a failure result when no cover with that id exists,
        /// or when claims still reference it.
        /// </summary>
        public async Task<Result> HandleAsync(DeleteCoverCommand command)
        {
            Cover? cover = await _covers.GetByIdAsync(command.Id);

            if (cover is null)
            {
                return Result.Failure(ResultErrorCodes.CoverNotFound);
            }

            // Since claim cannot be created without an existing cover, we can check if any claims reference this cover before deleting it
            bool hasClaims = await _claims.ExistsForCoverAsync(command.Id);

            if (hasClaims)
            {
                return Result.Failure(ResultErrorCodes.CoverHasClaims);
            }

            await _covers.RemoveAsync(cover);

            await _auditQueue.EnqueueAsync(new AuditMessage
            {
                EntityType = AuditEntityType.Cover,
                EntityId = command.Id,
                HttpRequestType = "DELETE",
                Created = DateTime.UtcNow
            });

            return Result.Success();
        }
    }
}
