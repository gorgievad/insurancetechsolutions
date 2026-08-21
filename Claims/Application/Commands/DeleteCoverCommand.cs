using Claims.Application.Common;
using Claims.Domain;
using Claims.Infrastructure;
using Claims.Infrastructure.Auditing;
using Microsoft.EntityFrameworkCore;

namespace Claims.Application.Commands
{
    public record DeleteCoverCommand(string Id);

    public class DeleteCoverCommandHandler
    {
        private readonly ClaimsContext _context;
        private readonly IAuditQueue _auditQueue;

        public DeleteCoverCommandHandler(ClaimsContext context, IAuditQueue auditQueue)
        {
            _context = context;
            _auditQueue = auditQueue;
        }

        /// <summary>
        /// Deletes the cover with the given id.
        /// Returns a failure result when no cover with that id exists,
        /// or when claims still reference it.
        /// </summary>
        public async Task<Result> HandleAsync(DeleteCoverCommand command)
        {
            Cover? cover = await _context.Covers.Where(c => c.Id == command.Id).SingleOrDefaultAsync();

            if (cover is null)
            {
                return Result.Failure(ResultErrorCodes.CoverNotFound);
            }

            // Since claim cannot be created without an existing cover, we can check if any claims reference this cover before deleting it
            bool hasClaims = await _context.Claims.AnyAsync(x => x.CoverId == command.Id);

            if (hasClaims)
            {
                return Result.Failure(ResultErrorCodes.CoverHasClaims);
            }

            _context.Covers.Remove(cover);
            await _context.SaveChangesAsync();

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
