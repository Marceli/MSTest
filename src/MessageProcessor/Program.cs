using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace MessageProcessor
{
	class Program
	{
		static void Main(string[] args)
		{
			int threadsNumber = 64;
			int messagesNumber = 100;
			var messageProcessor = new MessageProcessor(threadsNumber,messagesNumber);
			messageProcessor.Start();
			SetUpListeners();
			foreach (var histogramItem in messageProcessor.Histogram)
			{
				Trace.WriteLine(histogramItem.ToString());
			}
			Trace.WriteLine(string.Format("{0:0.000}",messageProcessor.AverageDispatches));
			Trace.WriteLine(string.Format("{0:00}:{1:00}:{2:00}.{3:000}",messageProcessor.ElapsedTime.Hours,messageProcessor.ElapsedTime.Minutes,messageProcessor.ElapsedTime.Seconds,messageProcessor.ElapsedTime.Milliseconds));
			Console.ReadKey();

		}

		private static void SetUpListeners()
		{
			Trace.Listeners.Clear();
			var fileName = "log.txt";
			if(File.Exists(fileName))
				File.Delete(fileName);

			var twtl = new TextWriterTraceListener(fileName);
			var ctl = new ConsoleTraceListener(false);
			Trace.Listeners.Add(twtl);
			Trace.Listeners.Add(ctl);
			Trace.AutoFlush = true;
		}
	}
}
