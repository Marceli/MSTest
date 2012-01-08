using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Marcel.MessageProcessor
{
	public class MessageProcessorWithCustomBlockingQueue : IMessageProcessor
	{
		private CountdownEvent countdown;
        private readonly ConcurrentBag<Message> results = new ConcurrentBag<Message>();
		private readonly int messagesCount;
		private readonly int threadsCount;
        private readonly CustomBlockingQueue<Message>[] toDispatch;
	    public IEnumerable<Message> Results
	    {
	        get
	        {
                Console.WriteLine("Using "+this.GetType().Name);
                countdown = new CountdownEvent(threadsCount * messagesCount);
                for (var i = 0; i < threadsCount; i++)
                {
                    // create local variable to do not access modified closure
                    var temp = i;
                    Task.Factory.StartNew(() => Dispatch(new MessageBuilder(threadsCount, messagesCount, temp).GetMessages(), temp), TaskCreationOptions.LongRunning);
                }
                countdown.Wait();
                toDispatch.ToList().ForEach(m=>m.CompleteAdding());
	            return results;
	        }
	    }

		public MessageProcessorWithCustomBlockingQueue(int threadsCount, int messagesCount)
		{
			ValidateParameters(threadsCount, messagesCount);
			this.threadsCount = threadsCount;
			this.messagesCount = messagesCount;
            toDispatch = new CustomBlockingQueue<Message>[threadsCount];
			
			this.threadsCount = threadsCount;
			for (var i = 0; i < threadsCount; i++)
			{				
				toDispatch[i] = new CustomBlockingQueue<Message>();
			}
		}

        public void Start()
        {
        }
        
        private void Dispatch(IEnumerable<Message> messages, int threadId)
        {
            //There is option to use ThreadSafeRandom class but creating separate instance for each thread seams to be 
            //cleaner solution.
            var random = new Random(threadId);
            messages.ToList().ForEach(toDispatch[threadId].Add);
          
            while (countdown.CurrentCount > 0)
            {
                Message message;
                if (!toDispatch[threadId].TryTake(out message))
                    continue;
                message.IncreaseDispatched();
                if (message.DestinationThreadId == threadId)
                {
                    //It's right message lets add it to result's collection for further analisis                   
                    results.Add(message);
                    //decrease global counter
                    countdown.Signal();
//                    Console.WriteLine("Found"+threadId+" CountDown"+countdown.CurrentCount);
                }
                else
                {
                    // that message should be dispatched by other thread lets put it to random bag
                    var randomThread = random.Next(0, threadsCount);
                    toDispatch[randomThread].Add(message);
                }
            }           
        }




	    public void ValidateParameters(int threadsNumber, int messagesNumber)
		{
			if (threadsNumber < 1 || threadsNumber > 1000)
				throw new ArgumentException("Number of threads must be beetween 1 and 1000", "threadsNumber");
			if (messagesNumber < 1 || messagesNumber > 1000)
				throw new ArgumentException("Number of messages must be beetween 1 and 1000", "messagesNumber");
		}

	    public IEnumerable<Message> GetResults
	    {
	        get { throw new NotImplementedException(); }
	    }
	}
}