
using System;

namespace MessageProcessor
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
                lock(locker)
                {
            		return despathes;
                }
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
			lock (locker)
			{
				despathes++;
			}
		}
	}
}