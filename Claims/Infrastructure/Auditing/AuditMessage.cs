namespace Claims.Infrastructure.Auditing
{
    public class AuditMessage
    {
        public AuditEntityType EntityType { get; set; }
        public string EntityId { get; init; } = string.Empty;
        public string HttpRequestType { get; init; } = string.Empty;
        public DateTime Created { get; init; } = DateTime.UtcNow;
    }
}
