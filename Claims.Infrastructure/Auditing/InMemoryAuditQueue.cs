using System.Threading.Channels;
using Claims.Application.Auditing;

namespace Claims.Infrastructure.Auditing
{
    public class InMemoryAuditQueue : IAuditQueue
    {
        private readonly Channel<AuditMessage> _channel;

        public InMemoryAuditQueue()
        {
            _channel = Channel.CreateUnbounded<AuditMessage>(new UnboundedChannelOptions
            {
                SingleReader = true,
                SingleWriter = false
            });
        }

        public ChannelReader<AuditMessage> Reader => _channel.Reader;

        public ValueTask EnqueueAsync(AuditMessage message)
        {
            return _channel.Writer.WriteAsync(message);
        }
    }
}
