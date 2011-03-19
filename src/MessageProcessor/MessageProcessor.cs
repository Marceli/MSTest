using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace Marcel.MessageProcessor
{
	public class MessageProcessor
	{
		private readonly int messagesCount;

		private readonly ThreadSafeRandom random = new ThreadSafeRandom();

		private readonly int threadsCount;

		private readonly IProducerConsumerCollection<Message> dispatched = new ConcurrentBag<Message>();

		private Stopwatch stopWatch;

		private readonly IProducerConsumerCollection<Message>[] toDispatch;

		public IProducerConsumerCollection<Message>[] ToDispatch
		{
			get { return toDispatch; }
		}

		private readonly CountdownEvent countdown;

		public MessageProcessor(int threadsCount, int messagesCount)
		{
			ValidateParameters(threadsCount, messagesCount);
			this.threadsCount = threadsCount;
			this.messagesCount = messagesCount;
			toDispatch = new IProducerConsumerCollection<Message>[threadsCount];
			countdown = new CountdownEvent(threadsCount*messagesCount);
			this.threadsCount = threadsCount;
			for (var i = 0; i < threadsCount; i++)
			{
				//No requirement to process messages in specific order hence ConcurrentBag should be fastest option
				toDispatch[i] = new ConcurrentBag<Message>();
			}
		}

		public void Start()
		{
			stopWatch = new Stopwatch();
			stopWatch.Start();
			for (var i = 0; i < threadsCount; i++)
			{
				// create local variable to do not access modified closure
				int temp = i;
				var worker = new Thread(() => Dispatch(new MessageBuilder(threadsCount, messagesCount).GetMessages(),temp))
				             	{Name = "MyWorker_" + temp};
				worker.Start();
				//I tried to use Task class but in test scenario x=64 n=256 apparently it was much slower than Thread.
				//Task.Factory.StartNew(()=>Dispatch(new MessageBuilder(threadsNumber, messagesCount).GetMessages(), temp));
			}
			countdown.Wait();
			stopWatch.Stop();
		}

		private void Dispatch(IEnumerable<Message> messages,int threadId)
		{
			foreach (Message message in messages)
			{
				toDispatch[threadId].TryAdd(message);
			}
			while (countdown.CurrentCount > 0)
			{
				Message message;

				if (!toDispatch[threadId].TryTake(out message))
				{
					//Looks like we processed all messages in our bag let give some time to other threads to fill it
					Thread.Sleep(1);
					continue;
				}
				message.IncreaseDispatched();
				if (message.DestinationThreadId == threadId)
				{
					//It's right message lets add it to result collection for further analisis
					if (!dispatched.TryAdd(message))
					{
						throw new Exception("Should never happen for ConcurrentBag");
					}
					//decrease counter
					countdown.Signal();
				}
				else
				{
					// that message should be dispatched by other thread lets put it to random bag
					int randomThread = ThreadSafeRandom.Next(0, threadsCount);
					if (!toDispatch[randomThread].TryAdd(message))
					{
						throw new Exception("Should never happen for ConcurrentBag");
					}
				}
			}
		}

		private void ValidateParameters(int threadsNumber, int messagesNumber)
		{
			if (threadsNumber < 1 || threadsNumber > 1000)
				throw new ArgumentException("Number of threads must be beetween 1 and 1000", "threadsNumber");
			if (messagesNumber < 1 || messagesNumber > 1000)
				throw new ArgumentException("Number of messages must be beetween 1 and 1000", "messagesNumber");
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
				       select new HistogramItem(grouped.Key,grouped.Count());
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
	}

}