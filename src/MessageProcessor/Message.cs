
using System;
using System.Threading;

namespace Marcel.MessageProcessor
{
	public class Message
	{
		private object locker=new object();
		private int despathes = 0;

		public Message(int destinationThreadId)
		{
			this.destinationThreadId = destinationThreadId;
		}

		private int destinationThreadId;

		public int Despathes
		{
			get
			{
        		return despathes;
			}
		}

		public int DestinationThreadId
		{
			get
			{
				return destinationThreadId;
			}
		}

		public void IncreaseDispatched()
		{
			Interlocked.Increment(ref this.despathes);
		}
	}
}