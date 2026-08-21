using Claims.Application.Auditing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Claims.Infrastructure.Auditing
{
    public class AuditBackgroundService : BackgroundService
    {
        private readonly IAuditQueue _queue;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<AuditBackgroundService> _logger;

        public AuditBackgroundService(IAuditQueue queue,
                                      IServiceScopeFactory scopeFactory,
                                      ILogger<AuditBackgroundService> logger)
        {
            _queue = queue;
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await foreach (AuditMessage msg in _queue.Reader.ReadAllAsync(stoppingToken))
            {
                try
                {
                    using IServiceScope scope = _scopeFactory.CreateScope();
                    AuditContext auditContext = scope.ServiceProvider.GetRequiredService<AuditContext>();

                    if (msg.EntityType == AuditEntityType.Claim)
                    {
                        ClaimAudit claimAudit = new ClaimAudit
                        {
                            ClaimId = msg.EntityId,
                            HttpRequestType = msg.HttpRequestType,
                            Created = DateTime.UtcNow
                        };
                        auditContext.Add(claimAudit);
                    }
                    else if (msg.EntityType == AuditEntityType.Cover)
                    {
                        CoverAudit coverAudit = new CoverAudit
                        {
                            CoverId = msg.EntityId,
                            HttpRequestType = msg.HttpRequestType,
                            Created = DateTime.UtcNow
                        };
                        auditContext.Add(coverAudit);
                    }

                    await auditContext.SaveChangesAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex,
                                     "Failed to write audit record for {EntityType} {EntityId} ({HttpRequestType}).",
                                     msg.EntityType,
                                     msg.EntityId,
                                     msg.HttpRequestType);
                }
            }
        }
    }
}
