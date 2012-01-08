using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Marcel;

namespace Marcel
{
	/// <summary>
	/// This class provides thread safe operations on internal Queue. It allso block thread that call TryTake
	/// when the queue is empty until new item is added.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class CustomBlockingQueue<T>
	{
		readonly object locker = new object();
		readonly Queue<T> queue = new Queue<T>();
		private bool completed;

		/// <summary>
		/// This method is needed to release blocked thread.
		/// </summary>
		public void CompleteAdding()
        {
            
			lock (locker)
			{
                this.completed = true;		
                //release blocking thread.
				Monitor.Pulse(locker);
			}
		}

		
		public void Add(T item)
		{
			lock (locker)
			{
				queue.Enqueue(item);
                //start thread waiting to consume the item.
				Monitor.Pulse(locker);		
			}			
		}
        
		public int Count
        {
            get
            {
//                lock (locker)
                    return queue.Count;
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