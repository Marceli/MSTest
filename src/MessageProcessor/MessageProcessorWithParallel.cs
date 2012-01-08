using System;
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
      
        private List<Message> results = new List<Message>();
        private readonly int messagesCount;
        private readonly int threadsCount;
        private readonly IList<Package> toDispatch=new List<Package>();

        public MessageProcessorWithParallel(int threadsCount, int messagesCount)
        {
            this.threadsCount = threadsCount;
            this.messagesCount = messagesCount;
            ValidateParameters();
			
            for (var i = 0; i < threadsCount; i++)
            {
                //No requirement to process messages in specific order hence ConcurrentBag should be fastest option
                toDispatch.Add( new Package(){ThreadId = i});
            }
        }
        public void Start()
        {
            Console.WriteLine("Using "+this.GetType().Name);
            Parallel.ForEach(toDispatch, Populate);
            countdown = new CountdownEvent(threadsCount * messagesCount);
            // var taskScheduler = new TaskScheduler().;
            //var cancellationTokenSource=new CancellationTokenSource();
           
            //var options = new ParallelOptions();
            //options.CancellationToken = cancellationTokenSource.Token;
            var backgroundWorker = new BackgroundWorker();
            backgroundWorker.DoWork+=new DoWorkEventHandler(backgroundWorker_DoWork);
            backgroundWorker.RunWorkerAsync();
            countdown.Wait();
          
            for (var i = 0; i < threadsCount; i++)
            {
                toDispatch[i].Messages.CompleteAdding();

            }
        }

        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            ThreadPool.SetMinThreads(threadsCount, threadsCount);
            toDispatch.AsParallel().ForAll(Populate);
            results=toDispatch.AsParallel().WithDegreeOfParallelism(threadsCount).SelectMany(p => GetResults(p)).ToList();
        }

        private IEnumerable<Message> GetResults(Package package)
        {
            var random = new Random(package.ThreadId);
            while (countdown.CurrentCount > 0)
            {
                foreach (var message in package.Messages.GetConsumingEnumerable())
                {
                    message.IncreaseDispatched();

                    if (message.DestinationThreadId == package.ThreadId)
                    {
                        yield return message;
                        countdown.Signal();
                    }
                    else
                    {
                        var randomThread = random.Next(0, threadsCount);
                        toDispatch[randomThread].Messages.TryAdd(message);
                    }
                }
                yield break;
            }
        }

        private void Populate(Package package)
        {
            var messages = new MessageBuilder(threadsCount, messagesCount, package.ThreadId).GetMessages();
            package.Messages=new BlockingCollection<Message>(new ConcurrentBag<Message>(messages));
        }

        public IEnumerable<Message> Results
        {
            get
            {
                return results;
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