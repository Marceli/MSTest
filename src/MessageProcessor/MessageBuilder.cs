using System.Collections.Generic;

namespace MessageProcessor
{
	public class MessageBuilder
	{
		private readonly int threadsNumber;
		private readonly int messagesNumber;

		public MessageBuilder(int threadsNumber, int messagesNumber)
		{
			this.threadsNumber = threadsNumber;
			this.messagesNumber = messagesNumber;
		}

		public IEnumerable<Message> GetMessages()
		{
			var result = new List<Message>();
			for (var i = 0; i < threadsNumber; i++)
			{
				result.Add(new Message(ThreadSafeRandom.Next(0, threadsNumber)));
			}
			return result;
		}
	}
}