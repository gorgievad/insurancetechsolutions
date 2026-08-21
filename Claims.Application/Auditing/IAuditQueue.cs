using System.Threading.Channels;

namespace Claims.Application.Auditing
{
    /// <summary>
    /// Port the application writes audit messages to.
    /// The queue itself and the worker that drains it live in the infrastructure layer.
    /// </summary>
    public interface IAuditQueue
    {
        ValueTask EnqueueAsync(AuditMessage message);
        ChannelReader<AuditMessage> Reader { get; }
    }
}
