using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;

namespace Marcel.MessageProcessor
{
    public class Package
    {
        public int ThreadId { get; set; }
        public BlockingCollection<Message> Messages;
    }
   
}