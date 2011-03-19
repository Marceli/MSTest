using System;

namespace Marcel.MessageProcessor
{
	public class HistogramItem
	{
		public HistogramItem(int dispatches, int dispatchesCount)
		{
			Dispatches = dispatches;
			DispatchesCount = dispatchesCount;
		}

		public int Dispatches { get; set; }

		public int DispatchesCount { get; set; }
		public override string ToString()
		{
			return string.Format("{0}    {1}", Dispatches, DispatchesCount);
		}
	}
}