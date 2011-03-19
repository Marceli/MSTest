using Marcel.MessageProcessor;
using NUnit.Framework;

namespace MessageProcessorTest
{
	[TestFixture]
	public class MessageTest
	{
		[Test]
		public void CanCreateMessage()
		{
			Assert.IsNotNull(new Message(1));
		}

		[Test]
		public void IncreaseDespatches_ShouldIncreaseDespatchesByOne()
		{
			var message = new Message(1);
			Assert.AreEqual(0, message.Despathes);
			message.IncreaseDispatched();
			Assert.AreEqual(1, message.Despathes);
		}
	}
}