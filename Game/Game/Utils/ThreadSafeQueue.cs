using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace MyGame.Utils
{
    public class ThreadSafeQueue<T>
    {
        private Mutex mutex;
        private Queue<T> queue;
        private Semaphore count;

        public ThreadSafeQueue()
        {
            mutex = new Mutex(false);
            count = new Semaphore(0, int.MaxValue);
            queue = new Queue<T>();
        }

        public void Enqueue(T obj)
        {
            mutex.WaitOne();
            queue.Enqueue(obj);
            count.Release();
            mutex.ReleaseMutex();
        }

        public T Dequeue()
        {
            count.WaitOne();
            mutex.WaitOne();
            T item = queue.Dequeue();
            mutex.ReleaseMutex();
            return item;
        }

        public Queue<T> DequeueAll()
        {
            mutex.WaitOne();
            Queue<T> rtn = new Queue<T>();

            while (count.WaitOne(0))
            {
                rtn.Enqueue(queue.Dequeue());
            }
            mutex.ReleaseMutex();
            return rtn;
        }

        public void EnqueueAll(Queue<T> q)
        {
            mutex.WaitOne();
            foreach (T item in q)
            {
                queue.Enqueue(item);
                count.Release();
            }
            mutex.ReleaseMutex();
        }

        public T Peek()
        {
            mutex.WaitOne();
            T item = queue.Peek();
            mutex.ReleaseMutex();
            return item;
        }

        public Boolean IsEmpty
        {
            get
            {
                Boolean rtn;
                mutex.WaitOne();
                rtn = queue.Count <= 0;
                mutex.ReleaseMutex();
                return rtn;
            }
        }

        public Boolean Contains(T obj)
        {
            mutex.WaitOne();
            Boolean rtn = queue.Contains(obj);
            mutex.ReleaseMutex();
            return rtn;
        }
    }
}
