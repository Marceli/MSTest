using System.Collections.Generic;

namespace Marcel.MessageProcessor
{
    public interface IMessageProcessor
    {
        IEnumerable<Message> Results { get; }
    }
}