using System;

namespace MessageProcessor
{
	public class HistogramItem
	{
		public int Dispatches { get; set; }

		public int DispatchesCount { get; set; }
		public override string ToString()
		{
			return string.Format("{0}    {1}", Dispatches, DispatchesCount);
		}
	}
}