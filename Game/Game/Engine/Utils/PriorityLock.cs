using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MyGame.Engine.Utils
{
    class PriorityLock
    {
        volatile object outerLock = new object();
        volatile object innerLock = new object();

        public void Enter()
        {
            bool lockTaken = false;
            Enter(ref lockTaken);
        }

        public void Enter(ref bool lockTaken)
        {
            do
            {
                Monitor.TryEnter(innerLock, ref lockTaken);
                if (!lockTaken)
                {
                    lock (outerLock)
                    {
                        Monitor.Wait(outerLock);
                    }
                }
                else
                {
                    return;
                }
            }
            while (!lockTaken);
        }

        public void PriorityEnter()
        {
            bool lockTaken = false;
            PriorityEnter(ref lockTaken);
        }

        public void PriorityEnter(ref bool lockTaken)
        {
            Monitor.Enter(innerLock, ref lockTaken);
        }

        public void Exit()
        {
            Monitor.Exit(innerLock);
            lock (outerLock)
            {
                Monitor.Pulse(outerLock);
            }
        }
    }
}
