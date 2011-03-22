using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Marcel.MessageProcessor
{
	public class MessageProcessor3
	{
      
		private readonly CountdownEvent countdown;
		private readonly ConcurrentBag<Message> results = new ConcurrentBag<Message>();
		private readonly int messagesCount;
		private readonly int threadsCount;
        private readonly BlockingCollection<Message>[] toDispatch;
		private Stopwatch stopWatch;

		public MessageProcessor3(int threadsCount, int messagesCount)
		{
			ValidateParameters(threadsCount, messagesCount);
			this.threadsCount = threadsCount;
			this.messagesCount = messagesCount;
            toDispatch = new BlockingCollection<Message>[threadsCount];
			countdown = new CountdownEvent(threadsCount*messagesCount);
			this.threadsCount = threadsCount;
			for (var i = 0; i < threadsCount; i++)
			{
				//No requirement to process messages in specific order hence ConcurrentBag should be fastest option
				toDispatch[i] = new BlockingCollection<Message>(new ConcurrentBag<Message>());
			}
		}

        public BlockingCollection<Message>[] ToDispatch
		{
			get { return toDispatch; }
		}

		public TimeSpan Elapsed
		{
			get
			{
				countdown.Wait();
				return stopWatch.Elapsed;
			}
		}

		public IEnumerable<string> Histogram
		{
			get
			{
                countdown.Wait();
                return from m in results
                       group m by m.Despathes
                           into grouped
                           orderby grouped.Key
                           select string.Format("{0}    {1}", grouped.Key, grouped.Count());
            }
		}

		public double AllDispatches
		{
			get
			{
				countdown.Wait();
				return (from m in results select m.Despathes).Sum();
			}
		}

		public double AverageDispatches
		{
			get
			{
				countdown.Wait();
				return (from m in results select m.Despathes).Average();
			}
		}

		public void Start()
		{
			stopWatch = new Stopwatch();
			stopWatch.Start();
            IList<ThreadParam> threadParams=new List<ThreadParam>();
			for (var i = 0; i < threadsCount; i++)
			{
				// create local variable to do not access modified closure
				var temp = i;
                var param=new ThreadParam(i, threadsCount, messagesCount);
                Task.Factory.StartNew(()=>Dispatch(param),TaskCreationOptions.LongRunning);
			}
			countdown.Wait();
			ReleaseAllThreads();
			stopWatch.Stop();
		}

		private void ReleaseAllThreads()
		{
    		toDispatch.ToList().ForEach(bc=>bc.CompleteAdding());
		}

		private void Dispatch(object threadParam)
		{
			//There is option to use ThreadSafeRandom class but creating separate instance for each thread seams to be 
			//cleaner solution.
            var threadId = ((ThreadParam)threadParam).ThreadId;
            var messages = ((ThreadParam)threadParam).Messages;
			var random=new Random(threadId);
			foreach (var message in messages)
			{
				toDispatch[threadId].Add(message);
			}
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
        
		private void ValidateParameters(int threadsNumber, int messagesNumber)
		{
			if (threadsNumber < 1 || threadsNumber > 1000)
                throw new ArgumentOutOfRangeException("Number of threads must be beetween 1 and 1000", "threadsNumber");
			if (messagesNumber < 1 || messagesNumber > 1000)
                throw new ArgumentOutOfRangeException("Number of messages must be beetween 1 and 1000", "messagesNumber");
		}
	}
    public class ThreadParam
    {
        int threadId;
        IEnumerable<Message> messages;
        public ThreadParam(int threadId,int threadsCount,int messagesCount)
        {
            this.threadId = threadId;
            messages=new MessageBuilder(threadsCount,messagesCount,threadId).GetMessages();
        }
        public IEnumerable<Message> Messages { get { return messages; } }
        public int ThreadId
        {
            get
            { 
                return threadId; 
            }
        }
    }
   
}