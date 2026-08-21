using Claims.Application.Auditing;
using Claims.Application.Persistence;
using Claims.Domain;
using Claims.Domain.Common;

namespace Claims.Application.Commands
{
    public record DeleteClaimCommand(string Id);

    public class DeleteClaimCommandHandler
    {
        private readonly IClaimRepository _claims;
        private readonly IAuditQueue _auditQueue;

        public DeleteClaimCommandHandler(IClaimRepository claims, IAuditQueue auditQueue)
        {
            _claims = claims;
            _auditQueue = auditQueue;
        }

        /// <summary>
        /// Deletes the claim with the given id.
        /// Returns a failure result when no claim with that id exists.
        /// </summary>
        public async Task<Result> HandleAsync(DeleteClaimCommand command)
        {
            Claim? claim = await _claims.GetByIdAsync(command.Id);

            if (claim is null)
            {
                return Result.Failure(ResultErrorCodes.ClaimNotFound);
            }

            await _claims.RemoveAsync(claim);

            await _auditQueue.EnqueueAsync(new AuditMessage
            {
                EntityType = AuditEntityType.Claim,
                EntityId = command.Id,
                HttpRequestType = "DELETE",
                Created = DateTime.UtcNow
            });

            return Result.Success();
        }
    }
}
