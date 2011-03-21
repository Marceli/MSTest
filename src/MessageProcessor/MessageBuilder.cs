using System;
using System.Collections.Generic;

namespace Marcel.MessageProcessor
{
	public class MessageBuilder
	{
		private readonly int threadsNumber;
		private readonly int messagesNumber;
		private readonly Random random;


		public MessageBuilder(int threadsNumber, int messagesNumber,int seed)
		{
			random=new Random(Environment.TickCount+seed);
			this.threadsNumber = threadsNumber;
			this.messagesNumber = messagesNumber;

		}

		public IEnumerable<Message> GetMessages()
		{
			var result = new List<Message>();
			for (var i = 0; i < messagesNumber; i++)
			{
				result.Add(new Message(random.Next(0, threadsNumber)));
			}
			return result;
		}
	}
}