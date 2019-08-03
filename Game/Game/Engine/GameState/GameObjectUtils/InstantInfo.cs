using MyGame.Engine.Utils;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGame.Engine.GameState.GameObjectUtils
{
    class InstantInfo
    {
        private ConcurrentDictionary<int, Info> info = new ConcurrentDictionary<int, Info>();

        public Info this[int i]
        {
            get
            {
                lock (info)
                {
                    if (info.ContainsKey(i))
                    {
                        return info[i];
                    }
                    else
                    {
                        Info newInfo = new Info();
                        info[i] = newInfo;
                        return newInfo;
                    }
                }
            }
        }

        public Info RemoveInstant(int i)
        {
            lock (info)
            {
                Info outValue;
                info.TryRemove(i, out outValue);
                return outValue;
            }
        }

        public bool ContainsInstant(int instantId)
        {
            return info.ContainsKey(instantId);
        }

        public bool IsInstantDeserialized(int instantId)
        {
            lock (info)
            {
                return info.ContainsKey(instantId) && info[instantId].IsDeserializde;
            }
        }


        internal class Info
        {
            private bool isDeserialized = false;
            private PriorityLock pLock = new PriorityLock();

            public bool IsDeserializde
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

            public PriorityLock Lock
            {
                get
                {
                    return pLock;
                }
            }
        }
    }
}
