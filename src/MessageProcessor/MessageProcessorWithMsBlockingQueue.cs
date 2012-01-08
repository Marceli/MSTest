using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Marcel.MessageProcessor
{
	public class MessageProcessorWithMsBlockingQueue:IMessageProcessor
	{
		private CountdownEvent countdown;
		private readonly ConcurrentBag<Message> results = new ConcurrentBag<Message>();
		private readonly int messagesCount;
		private readonly int threadsCount;
        private readonly IList<BlockingCollection<Message>> toDispatch;

		public MessageProcessorWithMsBlockingQueue(int threadsCount, int messagesCount)
		{
			ValidateParameters(threadsCount, messagesCount);
			this.threadsCount = threadsCount;
			this.messagesCount = messagesCount;
            toDispatch = new List<BlockingCollection<Message>>();
			
			this.threadsCount = threadsCount;
            //No requirement to process messages in specific order hence ConcurrentBag should be fastest option
            Enumerable.Range(0,threadsCount).ToList().ForEach(el=>toDispatch.Add(GetBlockingCollection(GetMessages(el))));
		}
        
		public IEnumerable<Message> GetMessages(int seed)
		{
		    var random =new Random(Environment.TickCount + seed);
		    for (var i = 0; i < messagesCount; i++)
		    {
		        yield return new Message(random.Next(0, threadsCount));
		    }
		}
        public BlockingCollection<Message> GetBlockingCollection(IEnumerable<Message> messages )
        {
            var col = new ConcurrentBag<Message>(messages);
            return new BlockingCollection<Message>(col);
        }

	    public IEnumerable<Message> Results
	    {
            get 
            { 
                Console.WriteLine("Using "+this.GetType().Name);
                countdown = new CountdownEvent(threadsCount * messagesCount);
                ThreadPool.SetMinThreads(threadsCount, threadsCount);
                var mytask=Task.Factory.StartNew(() => Parallel.ForEach(Enumerable.Range(0, threadsCount), Dispatch));
                countdown.Wait();
                toDispatch.ToList().ForEach(c=>c.CompleteAdding());
                mytask.Wait();
                return results;
            }
	    }


	    private void Dispatch(int threadId)
        {
            //There is option to use ThreadSafeRandom class but creating separate instance for each thread seams to be 
            //cleaner solution.
            var random = new Random(threadId);
            while (!toDispatch[threadId].IsCompleted)
            {
                foreach (var message in toDispatch[threadId].GetConsumingEnumerable())
                {
                    message.IncreaseDispatched();

                    if (message.DestinationThreadId == threadId)
                    {
                        //It's right message lets add it to result's collection for further analisis
                        results.Add(message);
                        //decrease global counter
                        countdown.Signal();
                    }
                    else
                    {
                        // that message should be dispatched by other thread lets put it to random bag
                        //Here could be used BlockingCollection.TryAddToAny but only but maximum 62 elements of array is suported.
                        var randomThread = random.Next(0, threadsCount);
                        toDispatch[randomThread].TryAdd(message);
                    }
                }
            }
        }

	    public void ValidateParameters(int threadsNumber, int messagesNumber)
		{
			if (threadsNumber < 1 || threadsNumber > 1000)
                throw new ArgumentOutOfRangeException("Number of threads must be beetween 1 and 1000", "threadsNumber");
			if (messagesNumber < 1 || messagesNumber > 1000)
                throw new ArgumentOutOfRangeException("Number of messages must be beetween 1 and 1000", "messagesNumber");
		}
	}
}