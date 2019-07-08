using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

//TODO: test this
namespace MyGame.Engine.DataStructures
{
    class ThreadSafeSortedList<KeyType, ValueType>
    {
        private Mutex mutex;
        private SortedList<KeyType, ValueType> list;
        private IComparer<KeyType> comparer;

        public ThreadSafeSortedList(IComparer<KeyType> comparer)
        {
            this.comparer = comparer;
            list = new SortedList<KeyType, ValueType>(comparer);
            mutex = new Mutex(false);
        }

        public void Add(KeyType key, ValueType value)
        {
            mutex.WaitOne();
            list.Add(key, value);
            mutex.ReleaseMutex();
        }

        public SortedList<KeyType, ValueType> PopList()
        {
            mutex.WaitOne();
            SortedList<KeyType, ValueType> returnList = list;
            list = new SortedList<KeyType, ValueType>(comparer);
            mutex.ReleaseMutex();
            return returnList;
        }
    }
}
