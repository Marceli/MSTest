using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;


namespace Marcel.MessageProcessor
{
	class Program
	{
		static IMessageProcessor messageProcessor;
		static TextWriterTraceListener textWriterTraceListener;
        const string fileName = "results.txt";
        static Stopwatch watch=new Stopwatch();
	    private static CountdownEvent myEvent;


	    static void Main(string[] args)
		{
			int threadsCount;
			int messagesCount;
//			if(args.Count()!=2 || !int.TryParse(args[0],out threadsCount)|| ! int.TryParse(args[1],out messagesCount))
//			{
//				Console.WriteLine("Please provide number of threads and number of messages");
//				Console.WriteLine("Usage : messageprocessor.exe 64 256");
//				return;
//			}
//			Console.WriteLine("It's O(T^2*M) (where T number of threads, M number of messages) algorithm so it can take some time to compute results for big T");
           
			threadsCount = 64;
			messagesCount = 256;
            //nice but doesn't handle no more than 63 threads how nice is that?
//			messageProcessor = new MessageProcessorWithParallel(threadsCount, messagesCount);
//            Start();
		//	messageProcessor = new MessageProcessorWithMsBlockingQueue(threadsCount, messagesCount);
         //   Start();
			messageProcessor = new MessageProcessorFastest(threadsCount, messagesCount);
            Start();
		    Console.ReadKey();
		}

	    private static void Start()
	    {
	        myEvent = new CountdownEvent(1);
            watch.Reset();
	        var t=Task.Factory.StartNew(DisplayResults);
	        while (!myEvent.IsSet)
	        {
	            Console.Write(".");
	            Thread.Sleep(500);
	        }
	        t.Wait();

	    }


		private static void DisplayResults()
		{
            watch.Reset();

            watch.Start();

		    var results = messageProcessor.Results;
            watch.Stop();
		    var histogram = results.GroupBy(m=>m.Despathes).OrderBy(g=>g.Key);
            SetUpListeners();
		    //histogram.Select(g => string.Format("{0}    {1}", g.Key, g.Count())).ToList().ForEach(PrintHistogram);
            Trace.WriteLine("Maximum despaches is:"+results.GroupBy(m=>m.Despathes).Max(g=>g.Key));
            Trace.WriteLine("Minimum despaches is:"+results.GroupBy(m=>m.Despathes).Min(g=>g.Key));
            Trace.WriteLine(string.Format("Avaerage despaches is:{0:0.000}",results.Average(m=>m.Despathes)));
            

            Trace.WriteLine(string.Format("Run time: {0:00}:{1:00}:{2:00}.{3:000}",
                watch.Elapsed.Hours,
                watch.Elapsed.Minutes,
                watch.Elapsed.Seconds,
                watch.Elapsed.Milliseconds));
            Console.WriteLine("Results are stored in {0} file.", fileName);
            textWriterTraceListener.Close();
		    myEvent.Signal();

		}
        private static void PrintHistogram(string line)
        {
            Trace.WriteLine(line);
        }


		private static void SetUpListeners()
		{
			Trace.Listeners.Clear();
			
			if(File.Exists(fileName))
				File.Delete(fileName);

			textWriterTraceListener = new TextWriterTraceListener(fileName);
			var ctl = new ConsoleTraceListener(false);
			Trace.Listeners.Add(textWriterTraceListener);
			Trace.Listeners.Add(ctl);
			Trace.AutoFlush = true;
			
		}
	}
}
