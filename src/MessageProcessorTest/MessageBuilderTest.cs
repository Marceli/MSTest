using Marcel.MessageProcessor;
using NUnit.Framework;
using System.Linq;

namespace MessageProcessorTest
{
	[TestFixture]
	public class MessageBuilderTest
	{
		[Test]
		public void CanCreateMessageBuilder()
		{
			Assert.IsNotNull(new MessageBuilder(1, 1));
		}
		[Test]
		public void GetMessages_WithOne_ReturnsOneMessage()
		{
			var messageBuilder = new MessageBuilder(1,1);
			Assert.AreEqual(1,messageBuilder.GetMessages().Count());
		}
		[Test]
		public void GetMessages_WithTen_ReturnsTenMessages()
		{
			var messageBuilder = new MessageBuilder(1, 10);
			Assert.AreEqual(10,messageBuilder.GetMessages().Count());
		}
	}
}