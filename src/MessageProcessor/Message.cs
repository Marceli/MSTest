
using System;
using System.Threading;

namespace Marcel.MessageProcessor
{
	public class Message
	{
        int despatches = 0;

		public Message(int destinationThreadId)
		{
            
			this.destinationThreadId = destinationThreadId;
		}

		private int destinationThreadId;

		public int Despathes
		{
			get
			{
                return despatches;
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
            Interlocked.Increment(ref despatches);
		}
	}
}