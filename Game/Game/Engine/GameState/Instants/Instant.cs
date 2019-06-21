using MyGame.Engine.Serialization;
using MyGame.Engine.Utils;
using System;
using System.Collections.Generic;

namespace MyGame.Engine.GameState.Instants
{
    class Instant
    {
        private class Comparer : IComparer<int>
        {
            public int Compare(int x, int y)
            {
                return x - y;
            }
        }

        private int instant;
        private SortedList<int, GameObject> deserializedObjects = new SortedList<int, GameObject>(new Comparer());
        private SortedList<int, GameObject> objects = new SortedList<int, GameObject>(new Comparer());
        private Logger log = new Logger(typeof(Instant));

        public Instant(int instant)
        {
            this.instant = instant;
        }

        public void AddDeserializedObject(GameObject obj)
        {
            if (obj.ID != null)
            {
                int id = (int)obj.ID;
                if (deserializedObjects.ContainsKey(id))
                {
                    log.Warn("An object that is already contained in the deserialized set is being added again.");
                }
                deserializedObjects.Add((int)obj.ID, obj);
            }
            else
            {
                throw new Exception("Objects in instants must have IDs");
            }
        }

        //TODO: can this get removed?
        public Instant(byte[] buffer, ref int bufferOffset)
        {
            instant = Serialization.Utils.ReadInt(buffer, ref bufferOffset);
        }

        public Instant GetNext
        {
            get
            {
                return new Instant(instant + 1);
            }
        }

        public CurrentInstant AsCurrent
        {
            get
            {
                return new CurrentInstant(this);
            }
        }

        public NextInstant AsNext
        {
            get
            {
                return new NextInstant(this);
            }
        }

        public int SerializationSize
        {
            get
            {
                return sizeof(int);
            }
        }

        public void Serialize(byte[] buffer, ref int bufferOffset)
        {
            if (buffer.Length - bufferOffset < SerializationSize)
            {
                throw new Exception("Buffer length does not match expected state length");
            }
            Serialization.Utils.Write(instant, buffer, ref bufferOffset);
        }

        public override bool Equals(object obj)
        {
            if(obj != null && obj is Instant)
            {
                return instant == ((Instant)obj).instant;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return instant;
        }
    }
}
