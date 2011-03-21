using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;


namespace Marcel.MessageProcessor
{
	class Program
	{
		static bool finished=false;
		static MessageProcessor2 messageProcessor;
		static TextWriterTraceListener textWriterTraceListener;
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
			Console.WriteLine(
				"It's O(T^2*M) (where T number of threads, M number of messages) algoritm so it can take some time to compute results for big T");
			threadsCount = 64;
			messagesCount = 256;

			messageProcessor = new MessageProcessor2(threadsCount, messagesCount);
			var backgroundWorker = new BackgroundWorker();
			backgroundWorker.RunWorkerCompleted+=DisplayResults;
			backgroundWorker.DoWork+=ProcessMessages;
			backgroundWorker.RunWorkerAsync();
			while (!finished)
			{
				Console.Write(".");
				Thread.Sleep(500);
			}
			SetUpListeners();
			Trace.WriteLine(string.Format("Run time: {0:00}:{1:00}:{2:00}.{3:000}",
				messageProcessor.Elapsed.Hours,
				messageProcessor.Elapsed.Minutes,
				messageProcessor.Elapsed.Seconds,
				messageProcessor.Elapsed.Milliseconds));
			textWriterTraceListener.Close();
			Console.ReadLine();
		}

		private static void ProcessMessages(object sender, DoWorkEventArgs e)
		{
			messageProcessor.Start();
		}

		private static void DisplayResults(object sender, RunWorkerCompletedEventArgs e)
		{
			finished = true;

		}

		private static void SetUpListeners()
		{
			Trace.Listeners.Clear();
			const string fileName = "log.txt";
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
