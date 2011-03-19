using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Marcel.MessageProcessor;
using NUnit.Framework;

namespace MessageProcessorTest
{
[TestFixture]
public class HistogramItemTest
	{
	[Test]
	public void CanCreateHistogramItem()
	{
		Assert.IsNotNull(new HistogramItem(1,1));
	}
	[Test]
	public void ToStringReturnsFormattedString()
	{
		var histogramItem = new HistogramItem(1,2);
		Assert.AreEqual("1    2", histogramItem.ToString());
	}
	}
}
