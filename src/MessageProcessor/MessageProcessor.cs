using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Marcel.MessageProcessor
{
	public class MessageProcessor
	{
		private readonly CountdownEvent countdown;
		private readonly IProducerConsumerCollection<Message> dispatched = new ConcurrentBag<Message>();
		private readonly object[] locks;
		private readonly int messagesCount;
		private readonly int threadsCount;
		private readonly IProducerConsumerCollection<Message>[] toDispatch;
		private Stopwatch stopWatch;

		public MessageProcessor(int threadsCount, int messagesCount)
		{
			ValidateParameters(threadsCount, messagesCount);
			this.threadsCount = threadsCount;
			this.messagesCount = messagesCount;
			toDispatch = new IProducerConsumerCollection<Message>[threadsCount];
			locks = new object[threadsCount];
			countdown = new CountdownEvent(threadsCount*messagesCount);
			this.threadsCount = threadsCount;
			for (var i = 0; i < threadsCount; i++)
			{
				locks[i] = new object();
				//No requirement to process messages in specific order hence ConcurrentBag should be fastest option
				toDispatch[i] = new ConcurrentBag<Message>();
			}
		}

		public IProducerConsumerCollection<Message>[] ToDispatch
		{
			get { return toDispatch; }
		}

		public TimeSpan ElapsedTime
		{
			get
			{
				countdown.Wait();
				return stopWatch.Elapsed;
			}
		}

		public IEnumerable<HistogramItem> Histogram
		{
			get
			{
				countdown.Wait();
				return from m in dispatched
				       group m by m.Despathes
				       into grouped orderby grouped.Key
				       select new HistogramItem(grouped.Key, grouped.Count());
			}
		}

		public double AllDispatches
		{
			get
			{
				countdown.Wait();
				return (from m in dispatched select m.Despathes).Sum();
			}
		}

		public double AverageDispatches
		{
			get
			{
				countdown.Wait();
				return (from m in dispatched select m.Despathes).Average();
			}
		}

		public void Start()
		{
			stopWatch = new Stopwatch();
			stopWatch.Start();
			for (var i = 0; i < threadsCount; i++)
			{
				// create local variable to do not access modified closure
				var temp = i;
				var worker = new Thread(() => Dispatch(new MessageBuilder(threadsCount, messagesCount,temp).GetMessages(), temp))
				             	{Name = "MyWorker_" + temp};
				worker.Start();
                // I tried to use TaskClass but apparently it was much slower than Thread
                //Task.Factory.StartNew(()=>Dispatch(new MessageBuilder(threadsCount, messagesCount,temp).GetMessages(), temp));
			}
			countdown.Wait();
			ReleaseAllThreads();
			stopWatch.Stop();
		}

		private void ReleaseAllThreads()
		{
			for (var i = 0; i < threadsCount; i++)
			{
				lock (locks[i])
					Monitor.Pulse(locks[i]);
			}
		}

		private void Dispatch(IEnumerable<Message> messages, int threadId)
		{
			//There is option to use ThreadSafeRandom class but creating separate instance for each thread seams to be 
			//cleaner solution.
			var random=new Random(threadId);
			foreach (var message in messages)
			{
				toDispatch[threadId].TryAdd(message);
			}
			while (countdown.CurrentCount > 0)
			{
				Message message;
            	if (!toDispatch[threadId].TryTake(out message))
            	{
            		//Looks like we processed all messages in our bag let give some time to other threads to fill it
    				lock (locks[threadId])
						Monitor.Wait(locks[threadId]);
            		continue;
            	}
                message.IncreaseDispatched();
				if (message.DestinationThreadId == threadId)
				{
					//It's right message lets add it to result's collection for further analisis
					AddToResultsCollection(message);
					//Console.WriteLine("Found"+threadId+" CountDown"+countdown.CurrentCount);
				}
				else
				{
					// that message should be dispatched by other thread lets put it to random bag
					PassToRandomThread(message,random);
				}
			}
		}

		private void AddToResultsCollection(Message message)
		{
			dispatched.TryAdd(message);
			//decrease global counter
			countdown.Signal();
		}

		private void PassToRandomThread(Message message,Random random)
		{
			var randomThread = random.Next(0, threadsCount);
            toDispatch[randomThread].TryAdd(message);
			lock (locks[randomThread])
				Monitor.Pulse(locks[randomThread]);
		}

		private void ValidateParameters(int threadsNumber, int messagesNumber)
		{
			if (threadsNumber < 1 || threadsNumber > 1000)
				throw new ArgumentException("Number of threads must be beetween 1 and 1000", "threadsNumber");
			if (messagesNumber < 1 || messagesNumber > 1000)
				throw new ArgumentException("Number of messages must be beetween 1 and 1000", "messagesNumber");
		}
	}
}