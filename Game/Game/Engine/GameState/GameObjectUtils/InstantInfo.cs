using MyGame.Engine.Utils;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MyGame.Engine.GameState.GameObjectUtils
{
    //TODO: can this performance be improved I don't lock on the dictionary
    class InstantInfo
    {
        private ConcurrentDictionary<int, Info> infoDict = new ConcurrentDictionary<int, Info>();

        public Info TryEnter(int i)
        {
            lock (infoDict)
            {
                if (infoDict.ContainsKey(i))
                {
                    Info info = infoDict[i];
                    bool lockTaken = false;
                    Monitor.TryEnter(info, ref lockTaken);
                    if (lockTaken)
                    {
                        return infoDict[i];
                    }
                }
                return null;
            }
        }

        public Info CreateAndTryEnter(int i)
        {
            lock (infoDict)
            {
                Info info;
                if (infoDict.ContainsKey(i))
                {
                    info = infoDict[i];
                }
                else
                {
                    info = new Info();
                    infoDict[i] = info;
                }
                bool lockTaken = false;
                Monitor.TryEnter(info, ref lockTaken);
                if (lockTaken)
                {
                    return infoDict[i];
                }
                else
                {
                    return null;
                }
            }
        }

        public void Exit(Info info)
        {
            Monitor.Exit(info);
        }

        //This deadlocks
        public Info RemoveInstant(int i)
        {
            lock (infoDict)
            {
                if (infoDict.ContainsKey(i))
                {
                    Info info = infoDict[i];
                    lock(info)
                    {
                        Info outValue;
                        infoDict.TryRemove(i, out outValue);
                        return info;
                    }
                }
                return null;
            }
        }

        internal class Info
        {
            private volatile bool isDeserialized = false;

            public bool IsDeserialized
            {
                get
                {
                    return isDeserialized;
                }

                set
                {
                    isDeserialized = value;
                }
            }
        }
    }
}
