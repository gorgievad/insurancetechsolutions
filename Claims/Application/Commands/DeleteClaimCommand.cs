using Claims.Application.Common;
using Claims.Domain;
using Claims.Infrastructure;
using Claims.Infrastructure.Auditing;
using Microsoft.EntityFrameworkCore;

namespace Claims.Application.Commands
{
    public record DeleteClaimCommand(string Id);

    public class DeleteClaimCommandHandler
    {
        private readonly ClaimsContext _context;
        private readonly IAuditQueue _auditQueue;

        public DeleteClaimCommandHandler(ClaimsContext context, IAuditQueue auditQueue)
        {
            _context = context;
            _auditQueue = auditQueue;
        }

        /// <summary>
        /// Deletes the claim with the given id.
        /// Returns a failure result when no claim with that id exists.
        /// </summary>
        public async Task<Result> HandleAsync(DeleteClaimCommand command)
        {
            Claim? claim = await _context.Claims.Where(c => c.Id == command.Id).SingleOrDefaultAsync();

            if (claim is null)
            {
                return Result.Failure(ResultErrorCodes.ClaimNotFound);
            }

            _context.Claims.Remove(claim);
            await _context.SaveChangesAsync();

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
