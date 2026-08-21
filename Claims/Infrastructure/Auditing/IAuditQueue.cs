using System.Threading.Channels;

namespace Claims.Infrastructure.Auditing
{
    public interface IAuditQueue
    {
        ValueTask EnqueueAsync(AuditMessage message);
        ChannelReader<AuditMessage> Reader { get; }
    }
}
