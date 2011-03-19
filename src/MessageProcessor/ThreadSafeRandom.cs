using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MessageProcessor
{
	public class ThreadSafeRandom
	{
		private static Random random=new Random(Environment.TickCount);
		public static int Next(int min,int max)
		{
			lock(random)
			{
				return random.Next(min, max);

			}
		}
	}
}
