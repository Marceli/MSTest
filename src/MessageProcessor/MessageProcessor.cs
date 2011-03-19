using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace MessageProcessor
{
	public class MessageProcessor
	{
		private readonly CountdownEvent countdown;
		private readonly IProducerConsumerCollection<Message> dispatched = new ConcurrentBag<Message>();
		private readonly int messagesNumber;
		private readonly ThreadSafeRandom random = new ThreadSafeRandom();
		private readonly int threadsNumber;
		private readonly IProducerConsumerCollection<Message>[] toDispatch;
		private Stopwatch stopWatch;

		public MessageProcessor(int threadsNumber, int messagesNumber)
		{
			this.threadsNumber = threadsNumber;
			this.messagesNumber = messagesNumber;
			ValidateParameters(threadsNumber, messagesNumber);
			toDispatch = new IProducerConsumerCollection<Message>[threadsNumber];
			countdown = new CountdownEvent(threadsNumber*messagesNumber);
			this.threadsNumber = threadsNumber;
			for (int i = 0; i < threadsNumber; i++)
			{
				toDispatch[i] = new ConcurrentBag<Message>();
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
				       select new HistogramItem {Dispatches = grouped.Key, DispatchesCount = grouped.Count()};
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

		public TimeSpan ElapsedTime
		{
			get
			{
				countdown.Wait();
				return stopWatch.Elapsed;
			}
		}

		public void Start()
		{
			stopWatch = new Stopwatch();
			stopWatch.Start();
			for (int i = 0; i < threadsNumber; i++)
			{
				int temp = i;
				var worker = new Thread(() => Dispatch(new MessageBuilder(threadsNumber, messagesNumber).GetMessages(), temp))
				             	{Name = "MyWorker_" + temp};
				worker.Start();
				//I tried to use Task class but in test scenario x=64 n=256 apparently it was much slower than Thread.
				//Task.Factory.StartNew(()=>Dispatch(new MessageBuilder(threadsNumber, messagesNumber).GetMessages(), temp));
			}
			countdown.Wait();
			stopWatch.Stop();
		}

		private void ValidateParameters(int threadsNumber, int messagesNumber)
		{
			if (threadsNumber < 1 || threadsNumber > 1000)
				throw new ArgumentException("Number of threads must be beetween 1 and 1000", "threadsNumber");
			if (messagesNumber < 1 || messagesNumber > 1000)
				throw new ArgumentException("Number of messages must be beetween 1 and 1000", "messagesNumber");
		}

		private void Dispatch(IEnumerable<Message> messages, int threadId)
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
					Thread.Sleep(1);
					continue;
				}
				message.IncreaseDispatched();
				if (message.DestinationThreadId == threadId)
				{
					if (!dispatched.TryAdd(message))
					{
						throw new Exception("Should never happen for ConCurrentBag");
					}
					countdown.Signal();
				}
				else
				{
					int randomThread = ThreadSafeRandom.Next(0, threadsNumber);
					if (!toDispatch[randomThread].TryAdd(message))
					{
						throw new Exception("Should never happen for ConCurrentBag");
					}
				}
			}
		}
	}
}