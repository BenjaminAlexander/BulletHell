using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace MyGame.Networking
{
    public class ThreadSafeQueue<T>
    {
        private Mutex mutex;
        private Queue<T> que;
        private Semaphore count;

        public ThreadSafeQueue()
        {
            mutex = new Mutex(false);
            count = new Semaphore(0, int.MaxValue);
            que = new Queue<T>();
        }

        public void Enqueue(T obj)
        {
            mutex.WaitOne();
            que.Enqueue(obj);
            count.Release();
            mutex.ReleaseMutex();
        }

        public T Dequeue()
        {
            mutex.WaitOne();
            T item = que.Dequeue();
            mutex.ReleaseMutex();
            return item;
        }

        public T Peek()
        {
            mutex.WaitOne();
            T item = que.Peek();
            mutex.ReleaseMutex();
            return item;
        }

        public Boolean IsEmpty
        {
            get
            {
                Boolean rtn;
                mutex.WaitOne();
                rtn = que.Count <= 0;
                mutex.ReleaseMutex();
                return rtn;
            }
        }

        public void WaitOn()
        {
            count.WaitOne();
        }
    }
}
