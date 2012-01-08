using System.Collections.Generic;

namespace Marcel.MessageProcessor
{
    public interface IMessageProcessor
    {
        void Start();
        IEnumerable<Message> Results { get; }
    }
}