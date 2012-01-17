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
		private IList<Message> results ;
		private readonly int messagesCount;
		private readonly int threadsCount;
        private readonly BlockingCollection<Message>[] toDispatch;

		public MessageProcessorWithMsBlockingQueue(int threadsCount, int messagesCount)
		{
			ValidateParameters(threadsCount, messagesCount);
			this.threadsCount = threadsCount;
			this.messagesCount = messagesCount;
            toDispatch = new BlockingCollection<Message>[threadsCount];
			
			this.threadsCount = threadsCount;
            //No requirement to process messages in specific order hence ConcurrentBag should be fastest option
//            Enumerable.Range(0,threadsCount).ToList().ForEach(el=>toDispatch.Add(GetBlockingCollection(GetMessages(el))));
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
            var col = new ConcurrentQueue<Message>(messages);
            return new BlockingCollection<Message>(col);
        }

	    public class DispatchPackage
	    {
	        private readonly int id;
	        private readonly BlockingCollection<Message> coll;

	        public int Id
	        {
	            get { return id; }
	        }

	        public BlockingCollection<Message> Coll
	        {
	            get { return coll; }
	        }

	        public DispatchPackage(int id, BlockingCollection<Message> coll)
	        {
	            this.id = id;
	            this.coll = coll;
	        }
	    }

	    public IEnumerable<Message> Results
	    {
            get 
            { 
                Console.WriteLine("Using "+this.GetType().Name);
                countdown = new CountdownEvent(threadsCount * messagesCount);
                ThreadPool.SetMinThreads(threadsCount, threadsCount);
                    Enumerable.Range(0, threadsCount).AsParallel().WithDegreeOfParallelism(63).ForAll(
                        el => toDispatch[el] = GetBlockingCollection(GetMessages(el)));
                var olo=Enumerable.Range(0, threadsCount).AsParallel().WithDegreeOfParallelism(63).SelectMany(Dispatch);
                var bolo = Task<List<Message>>.Factory.StartNew(olo.ToList);
                countdown.Wait();
                toDispatch.ToList().ForEach(c=>c.CompleteAdding());
                bolo.Wait();
                results=bolo.Result;
                return results;
            }
	    }



	    private IEnumerable<Message> Dispatch(int dp)
        {
            //There is option to use ThreadSafeRandom class but creating separate instance for each thread seams to be 
            //cleaner solution.
            var random = new Random(dp);
            Message message;
            while (!toDispatch[dp].IsCompleted)
            {
                if(toDispatch[dp].TryTake(out message,1))
                {
                    
                    message.IncreaseDispatched();

                    if (message.DestinationThreadId == dp)
                    {
//                        Console.WriteLine(countdown.CurrentCount);
                        //It's right message lets add it to result's collection for further analisis
                        countdown.Signal();
                        yield return message;
                        //decrease global counter
                    }
                    else
                    {
                        // that message should be dispatched by other thread lets put it to random bag
                        //Here could be used BlockingCollection.TryAddToAny but only but maximum 62 elements of array is suported.
                        var randomThread = random.Next(0, threadsCount);
                        toDispatch[randomThread].TryAdd(message);
                    }
                }
                else
                {
                    Thread.Sleep(2);
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