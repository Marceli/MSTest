using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;


namespace Marcel.MessageProcessor
{
	class Program
	{
		static bool finished=false;
		static IMessageProcessor messageProcessor;
		static TextWriterTraceListener textWriterTraceListener;
        const string fileName = "results.txt";
        static Stopwatch watch=new Stopwatch();
        


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
           
			threadsCount = 63;
			messagesCount = 256;
            Console.WriteLine("using paralelFor collection");
            //nice but doesn't handle more than 63 threads how nice is that.
			Start(messageProcessor = new MessageProcessorWithParallel(threadsCount, messagesCount));
            Console.WriteLine("using ms blocking collection");
			Start(messageProcessor = new MessageProcessorWithMsBlockingQueue(threadsCount, messagesCount));
            Console.WriteLine("using custom collection");
			Start(messageProcessor = new MessageProcessorWithCustomBlockingQueue(threadsCount, messagesCount));
		    Console.ReadKey();
		}

	    private static void Start(IMessageProcessor processor)
	    {
	        finished = false;
            watch.Reset();
	        var backgroundWorker = new BackgroundWorker();
	        backgroundWorker.RunWorkerCompleted += DisplayResults;
	        watch.Start();
	        var arg = new DoWorkEventArgs(processor);
	        backgroundWorker.DoWork += ProcessMessages;
	        backgroundWorker.RunWorkerAsync(arg);
	        while (!finished)
	        {
	            Console.Write(".");
	            Thread.Sleep(500);
	        }
	    }

	    private static void ProcessMessages(object sender, DoWorkEventArgs e)
		{
            var olo=((DoWorkEventArgs)e.Argument);
	         ((IMessageProcessor) olo.Argument).Start();
		
		}

		private static void DisplayResults(object sender, RunWorkerCompletedEventArgs e)
		{
            watch.Stop();
		    var histogram = messageProcessor.Results.GroupBy(m=>m.Despathes).OrderBy(g=>g.Key);
			finished = true;
            SetUpListeners();
            Trace.WriteLine(string.Format("{0:0.000}",messageProcessor.Results.Average(m=>m.Despathes)));
		    //histogram.Select(g => string.Format("{0}    {1}", g.Key, g.Count())).ToList().ForEach(PrintHistogram);
            

            Trace.WriteLine(string.Format("Run time: {0:00}:{1:00}:{2:00}.{3:000}",
                watch.Elapsed.Hours,
                watch.Elapsed.Minutes,
                watch.Elapsed.Seconds,
                watch.Elapsed.Milliseconds));
            Console.WriteLine("Results are stored in {0} file.", fileName);
            textWriterTraceListener.Close();

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
