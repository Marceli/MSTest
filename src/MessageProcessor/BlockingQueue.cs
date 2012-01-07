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

        Queue<T> queue = new Queue<T>();
        private bool completed;
        bool lockTaken = false;
        SpinLock _spinLock = new SpinLock();
        SpinWait sw = new SpinWait();
        public void CompleteAdding()
        {
            completed = true;
            if (lockTaken) _spinLock.Exit(false);
        }

        public void Add(T item)
        {
            bool lockTaken = false;
            try
            {

                _spinLock.Enter(ref lockTaken);
                queue.Enqueue(item);
                sw.Reset();
            }
            finally
            {
                if (lockTaken) _spinLock.Exit(false);

            }


        }
        public int Count
        {
            get
            {
                try
                {
                    _spinLock.Enter(ref lockTaken);
                    return queue.Count;
                }
                finally
                {
                    if (lockTaken) _spinLock.Exit(false);

                }

            }
        }

        public bool TryTake(out T item)
        {
            
            try
            {
                bool lockTaken = false;
                _spinLock.Enter(ref lockTaken);
               
                while (queue.Count == 0 && !completed)
                {

                    sw.SpinOnce();
                    

                }
                item = queue.Dequeue();


            }
            finally
            {
                if (lockTaken) _spinLock.Exit(false);

            }

            //if (!completed)
            //    item = queue.Dequeue();
            //else
            //{
            //    item = default(T);
            //    return false;
            //}
            return true;


        }


    }
}

