using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Marcel.MessageProcessor;

namespace MessageProcessor
{
	public class BlockingQueue<T>
	{
		readonly object locker = new object();
		Queue<T> queue = new Queue<T>();
		private bool completed;

		public void CompleteAdding()
        {
            
			lock (locker)
			{
                this.completed = true;		
                //release lock
				Monitor.Pulse(locker);
			}
		}

		public void Add(T item)
		{
			lock (locker)
			{
				queue.Enqueue(item);
                //notify that we added item
				Monitor.Pulse(locker);		
			}			
		}
        public int Count
        {
            get
            {
                lock (locker)
                {

                    return queue.Count;

                }
            }
        }

		public bool TryTake(out T item)
		{
			lock (locker)
			{			
                while (queue.Count == 0 && !completed)
                {
                    // The queue is empty wait for Add and Pulse
                    //Release the lock on an object and blocks this thread
                    Monitor.Wait(locker);
                }    
				if(!completed)
					item = queue.Dequeue();
				else
				{
					item = default(T);
					return false;
				}
				return true;
			}
		}


	}
}
