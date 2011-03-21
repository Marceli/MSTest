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
	public class MyProducerQueue<T>:IProducerConsumerCollection<T>
	{
		readonly object listLock = new object();
		Queue<T> queue = new Queue<T>();
		private bool finished;

		public void Abort()
		{
			this.finished = true;
			lock (listLock)
			{
				Monitor.Pulse(listLock);
			}
		}

		public IEnumerator<T> GetEnumerator()
		{
			throw new NotImplementedException();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public void CopyTo(Array array, int index)
		{
			throw new NotImplementedException();
		}

		public int Count
		{
			get { throw new NotImplementedException(); }
		}

		public object SyncRoot
		{
			get { throw new NotImplementedException(); }
		}

		public bool IsSynchronized
		{
			get { throw new NotImplementedException(); }
		}

		public void CopyTo(T[] array, int index)
		{
			throw new NotImplementedException();
		}

		public bool TryAdd(T item)
		{
			lock (listLock)
			{
				queue.Enqueue(item);

				// We always need to pulse, even if the queue wasn't
				// empty before. Otherwise, if we add several items
				// in quick succession, we may only pulse once, waking
				// a single thread up, even if there are multiple threads
				// waiting for items.            
				Monitor.Pulse(listLock);
				
			}
			return true;
		}

		public bool TryTake(out T item)
		{
			lock (listLock)
			{
				// If the queue is empty, wait for an item to be added
				// Note that this is a while loop, as we may be pulsed
				// but not wake up before another thread has come in and
				// consumed the newly added object. In that case, we'll
				// have to wait for another pulse.
				while (queue.Count == 0 && !finished)
				{
					// This releases listLock, only reacquiring it
					// after being woken up by a call to Pulse
					Monitor.Wait(listLock);
				}
				if(!finished)
					item = queue.Dequeue();
				else
				{
					item = default(T);
					return false;
				}
				return true;
			}
		}

		public T[] ToArray()
		{
			throw new NotImplementedException();
		}
	}
}
