using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyGame.Engine.DataStructures;
using MyGame.Engine.Reflection;


namespace MyGame.Engine.Serialization
{
    class DeserializedCollection<BaseType> : ICollection<BaseType> where BaseType : Deserializable
    {
        int nextID = 0;
        TwoWayMap<int, BaseType> map = new TwoWayMap<int, BaseType>();
        TypeDeserializer<BaseType> typeSerializer;

        public int Count
        {
            get
            {
                return map.Count;
            }
        }

        bool ICollection<BaseType>.IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public DeserializedCollection(TypeDeserializer<BaseType> typeSerializer)
        {
            this.typeSerializer = typeSerializer;
        }

        public DeserializedCollection(TypeFactory<BaseType> factory) : this(new TypeDeserializer<BaseType>(factory))
        {

        }

        public virtual void Add(BaseType obj)
        {
            if (!map.ContainsValue(obj))
            {
                int id = nextID;
                nextID++;
                map.Set(id, obj);
            }
        }

        public bool Remove(BaseType obj)
        {
            return map.RemoveValue(obj);
        }

        public BaseType Deserialize(byte[] buffer)
        {
            int bufferOffset = 0;
            return this.Deserialize(buffer, ref bufferOffset);
        }

        public BaseType Deserialize(byte[] buffer, ref int bufferOffset)
        {
            int objectId = Utils.ReadInt(buffer, ref bufferOffset);
            if (map.ContainsKey(objectId))
            {
                typeSerializer.Deserialize(map[objectId], buffer, ref bufferOffset);
                return map[objectId];
            }
            else
            {
                BaseType newObject = typeSerializer.Deserialize(buffer, ref bufferOffset);
                map.Set(objectId, newObject);
                return newObject;
            }
        }

        public void Clear()
        {
            nextID = 0;
            map.Clear();
        }

        public bool Contains(BaseType item)
        {
            return map.ContainsValue(item);
        }

        void ICollection<BaseType>.CopyTo(BaseType[] array, int arrayIndex)
        {
            if (array == null)
            {
                throw new ArgumentNullException();
            }

            if (arrayIndex < 0)
            {
                throw new ArgumentOutOfRangeException();
            }

            if (array.Length - arrayIndex < Count)
            {
                throw new ArgumentException();
            }

            for (int i = 0; i < Count; i++) ;

        }

        IEnumerator<BaseType> IEnumerable<BaseType>.GetEnumerator()
        {
            return map.Values.GetEnumerator();
        }


        IEnumerator IEnumerable.GetEnumerator()
        {
            return map.Values.GetEnumerator();
        }

        protected int GetObjectID(BaseType obj)
        {
            return map.GetKey(obj);
        }
    }
}
