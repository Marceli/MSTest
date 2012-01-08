using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Marcel.MessageProcessor
{
    public class MessageProcessorWithParallel:IMessageProcessor
    {
        private CountdownEvent countdown;
        private IList<Message> results = new List<Message>();
        private readonly int messagesCount;
        private readonly int threadsCount;
        private readonly IList<Package> toDispatch=new List<Package>();

        public MessageProcessorWithParallel(int threadsCount, int messagesCount)
        {
            this.threadsCount = threadsCount;
            this.messagesCount = messagesCount;
            ValidateParameters();
        }


        public IEnumerable<Message> Results
        {
            get
            {
                Console.WriteLine("Using " + this.GetType().Name);
                toDispatch.Clear();
                Enumerable.Range(0, threadsCount).ToList().
                    ForEach(i => toDispatch.Add(new Package
                                                    {
                                                        ThreadId = i,
                                                        Messages =GetMessagesCollection(i)
                                                    }));
                countdown = new CountdownEvent(threadsCount*messagesCount);
                ThreadPool.SetMinThreads(threadsCount, threadsCount);
                var myTask =Task<IList<Message>>.Factory.StartNew(() => toDispatch.AsParallel().WithDegreeOfParallelism(threadsCount).SelectMany(ProcessPackage).ToList());
                countdown.Wait();
                foreach (var package in toDispatch)
                {
                    package.Messages.CompleteAdding();
                }
                myTask.Wait();
                return myTask.Result;
            }
        }

		public BlockingCollection<Message> GetMessagesCollection(int seed)
		{
		    var collection =new ConcurrentBag<Message>();
		    var random =new Random(Environment.TickCount + seed);
		    for (var i = 0; i < messagesCount; i++)
		    {
		        collection.Add(new Message(random.Next(0, threadsCount)));
		    }
            return new BlockingCollection<Message>(collection);

		}

        private IEnumerable<Message> ProcessPackage(Package package)
        {
            var random = new Random(package.ThreadId);
            while (countdown.CurrentCount > 0)
            {
                foreach (var message in package.Messages.GetConsumingEnumerable())
                {
                    message.IncreaseDispatched();
                    if (message.DestinationThreadId == package.ThreadId)
                    {
                        countdown.Signal();
                        yield return message;
                    }
                    else
                    {
                        toDispatch[random.Next(0, threadsCount)].Messages.TryAdd(message);
                    }
                }
            }
        }



        public void ValidateParameters()
        {
            if (threadsCount < 1 || threadsCount > 1000)
                throw new ArgumentOutOfRangeException("Number of threads must be beetween 1 and 1000", "threadsNumber");
            if (messagesCount < 1 || messagesCount > 1000)
                throw new ArgumentOutOfRangeException("Number of messages must be beetween 1 and 1000", "messagesNumber");
        }

    
    }
}