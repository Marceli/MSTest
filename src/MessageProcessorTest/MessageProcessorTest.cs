using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Marcel.MessageProcessor;
using NUnit.Framework;
namespace MessageProcessorTest
{
	[TestFixture]
	public class MessageProcessorTest
	{
		[Test]
		public void CanCreateMessageProcessor()
		{
			Assert.IsNotNull(new MessageProcessor(1, 1));
		}

		[Test]
		public void HistogramItemDispathesCount_ForOneThread_IsEqualToMessagesCount()
		{
			var messageProcessor = new MessageProcessor(1,10);
			messageProcessor.Start();
			Assert.AreEqual(1,messageProcessor.Histogram.ElementAt(0).Dispatches);
			Assert.AreEqual(10,messageProcessor.Histogram.ElementAt(0).DispatchesCount);
		}

		[Test]
		public void AverageDispatches_ForOneThread_ReturnsOne()
		{
			var messageProcessor = new MessageProcessor(1,10);
			messageProcessor.Start();
			Assert.AreEqual(1,messageProcessor.AverageDispatches);

		}

		[Test, ExpectedException(typeof(ArgumentException))]
		public void Validate_WithMoreThanThousandThreads_ThrowsException()
		{
			var messageProcessor = new MessageProcessor(1001, 1);
		}

		[Test]
		public void Histogram_ForOneThread_ContainsOneItem()
		{
			var messageProcessor = new MessageProcessor(1,10);
			messageProcessor.Start();
			Assert.AreEqual(1,messageProcessor.Histogram.Count());

		}

		[Test]
		public void Validate_forValidParameters_doesNotThrow()
		{
			var minMessageProcessor = new MessageProcessor(1, 1);
			var maxMessageProcessor = new MessageProcessor(1000, 1000);

		}
		[Test, ExpectedException(typeof(ArgumentException))]
		public void Validate_WithZeroMessages_ThrowsException()
		{
			var messageProcessor = new MessageProcessor(1, 0);
		}

		[Test, ExpectedException(typeof(ArgumentException))]
		public void Validate_WithZeroThreads_ThrowsException()
		{
			var messageProcessor = new MessageProcessor(0, 1);
		}

		[Test, ExpectedException(typeof(ArgumentException))]
		public void Validate_WithMoreThanThousandMessages_ThrowsException()
		{
    		var messageProcessor=new MessageProcessor(1,1001);
		}

		[Test]
		public void MessageProcessor_ForNTthreads_createsNElementsSizeToDispatchArray()
		{
			var messageProcessor = new MessageProcessor(10,1);
			Assert.AreEqual(10,messageProcessor.ToDispatch.Count());
			Assert.IsNotNull(messageProcessor.ToDispatch[0]);
		}
	}
}
