using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Marcel.MessageProcessor
{
/// <summary>
/// Random class is not thread safe (it starts returns 0 after a while) this class ThreadSafeRandom will address this issue.
/// </summary>
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
