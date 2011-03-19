using Marcel.MessageProcessor;
using NUnit.Framework;

namespace MessageProcessorTest
{
	[TestFixture]
	public class ThreadSafeRandomTest
	{
		[Test]
		public void ThreadSafeRandom_ForMinEqualMax_ReturnsMin()
		{
			Assert.AreEqual(0, ThreadSafeRandom.Next(0, 0));
		}
	}
}